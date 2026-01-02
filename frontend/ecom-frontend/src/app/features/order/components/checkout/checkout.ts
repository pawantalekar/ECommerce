import { Component, NgModule, OnInit, inject } from '@angular/core';
import { AddressService } from '../../../shared/services/address-service';
import { AddressDto } from '../../../shared/models/address-model';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, NgModel, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Orderservice } from '../../services/orderservice';
import { CheckoutItem, ShippingAddress } from '../../models/order';
import { InitiatePaymentResponse } from '../../models/payment';
import { CartHttpService } from '../../../cart/services/cart-http.service';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface CheckoutDisplayItem {
  productId: string;
  productName: string;
  thumbnailUrl?: string;
  unitPrice: number;
  quantity: number
}

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './checkout.html'
})
export class CheckoutComponent implements OnInit {
  private fb = inject(FormBuilder);
  private orderService = inject(Orderservice);
  private router = inject(Router);
  private cartService = inject(CartHttpService);
  private http = inject(HttpClient);

  form: FormGroup;
  cartItems: CheckoutDisplayItem[] = [];
  totalAmount = 0;

  addressService = inject(AddressService);
  savedAddresses: AddressDto[] = [];
  addressesLoading = true;
  addressesError?: string;
  selectedAddressId: string | null = null;
  showAddressForm = false;
  isEditMode = false;

  constructor() {
    this.form = this.fb.group({
      fullName: ['', Validators.required],
      phone: ['', Validators.required],
      addressLine1: ['', Validators.required],
      addressLine2: [''],
      city: ['', Validators.required],
      state: ['', Validators.required],
      pincode: ['', Validators.required],
      shippingCountry: ['', Validators.required],
      shippingAddressType: ['', Validators.required]
    });
  }

  selectAddress(addr: AddressDto) {
    this.selectedAddressId = addr.id;
    this.showAddressForm = false;
  }

  confirmDeliverHere(addr: AddressDto) {
    this.selectedAddressId = addr.id;
    this.showAddressForm = false;
    // Only mark as selected, do not proceed to payment
  }

  onChangeAddress() {
    this.selectedAddressId = null;
    this.showAddressForm = false;
  }

  onAddNewAddress() {
    this.showAddressForm = true;
    this.isEditMode = false;
    this.form.reset();
  }

  onCancelAddressForm() {
    this.showAddressForm = false;
    this.isEditMode = false;
  }

  onSaveAndDeliverAddress() {
    if (this.form.invalid) return;
    const newAddress = {
      fullName: this.form.value.fullName,
      phone: this.form.value.phone,
      addressLine1: this.form.value.addressLine1,
      addressLine2: this.form.value.addressLine2,
      city: this.form.value.city,
      state: this.form.value.state,
      pincode: this.form.value.pincode,
      country: this.form.value.shippingCountry,
      addressType: this.form.value.shippingAddressType,
      isDefault: false
    };
    this.addressService.addMyAddress(newAddress).subscribe({
      next: (added) => {
        this.selectedAddressId = added.id;
        this.showAddressForm = false;
        this.isEditMode = false;
        this.loadSavedAddresses();
      }
    });
  }

  onEditAddress(addr: AddressDto) {
    this.showAddressForm = true;
    this.isEditMode = true;
    this.selectedAddressId = null;
    this.form.patchValue({
      fullName: addr.fullName,
      phone: addr.phone,
      addressLine1: addr.addressLine1,
      addressLine2: addr.addressLine2,
      city: addr.city,
      state: addr.state,
      pincode: addr.pincode,
      shippingCountry: addr.country,
      shippingAddressType: addr.addressType
    });
  }

  ngOnInit() {
    this.loadSavedAddresses();
    const state = history.state;

    if (state?.directBuyFromOrder) {
      this.cartItems = state.items;
      this.calculateTotal();
    }
    else if (state?.directBuy) {
      this.loadDirectBuyItem(state);
    }
    else {
      this.loadCartItems();
    }
  }

  loadSavedAddresses() {
    this.addressesLoading = true;
    this.addressService.getMyAddresses().subscribe({
      next: (addresses) => {
        this.savedAddresses = addresses;
        this.addressesLoading = false;
      },
      error: () => {
        this.addressesError = 'Failed to load saved addresses.';
        this.addressesLoading = false;
      }
    });
  }

  private loadCartItems() {
    this.cartService.getCart().subscribe(cart => {
      this.cartItems = cart.items.map(i => ({
        productId: i.productId,
        productName: i.name,
        thumbnailUrl: i.thumbnailUrl ?? undefined,
        unitPrice: i.price,
        quantity: i.quantity
      }));
      this.calculateTotal();
    });
  }

  private loadDirectBuyItem(state: any) {
    this.cartItems = [{
      productId: state.productId,
      productName: state.productName,
      thumbnailUrl: state.thumbnailUrl,
      unitPrice: state.price,
      quantity: state.quantity || 1
    }];
    this.calculateTotal();
  }

  private calculateTotal() {
    this.totalAmount = this.cartItems.reduce((sum, i) => sum + i.unitPrice * i.quantity, 0);
  }

  onPay() {
    if (this.selectedAddressId) {
      const selected = this.savedAddresses.find(a => a.id === this.selectedAddressId);
      if (!selected) return;
      const shipping: ShippingAddress = {
        fullName: selected.fullName,
        phone: selected.phone,
        addressLine1: selected.addressLine1,
        addressLine2: selected.addressLine2,
        city: selected.city,
        state: selected.state,
        pincode: selected.pincode,
        shippingCountry: selected.country,
        shippingAddressType: selected.addressType
      };
      const itemsForOrder: CheckoutItem[] = this.cartItems.map(i => ({
        productId: i.productId,
        quantity: i.quantity
      }));
      this.orderService.createOrder(itemsForOrder, shipping).subscribe((res: InitiatePaymentResponse) => {
        this.openRazorpay(res);
      });
    } else {
      // Use form values
      if (this.form.invalid) return;
      const itemsForOrder: CheckoutItem[] = this.cartItems.map(i => ({
        productId: i.productId,
        quantity: i.quantity
      }));
      const shipping = this.form.value as ShippingAddress;
      this.orderService.createOrder(itemsForOrder, shipping).subscribe((res: InitiatePaymentResponse) => {
        this.openRazorpay(res);
      });
    }
  }
  updateQty(index: number, change: number) {
    const newQty = this.cartItems[index].quantity + change;
    if (newQty >= 1) {
      this.cartItems[index].quantity = newQty;
      this.calculateTotal();
    }
  }
  openRazorpay(res: InitiatePaymentResponse) {
    const script = document.createElement('script');
    script.src = 'https://checkout.razorpay.com/v1/checkout.js';
    script.async = true;
    document.body.appendChild(script);

    script.onload = () => {
      const options = {
        key: res.keyId,
        amount: res.amount * 100,
        currency: 'INR',
        order_id: res.razorpayOrderId,
        handler: () => this.router.navigate(['/order/success']),
        modal: { ondismiss: () => this.router.navigate(['/order/failed']) }
      };
      const rzp = new (window as any).Razorpay(options);
      rzp.open();
    };
  }
}