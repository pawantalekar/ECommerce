import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Orderservice } from '../../services/orderservice';
import { CheckoutItem, ShippingAddress } from '../../models/order';
import { InitiatePaymentResponse } from '../../models/payment';
import { CartHttpService } from '../../../cart/services/cart-http.service';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

interface CheckoutDisplayItem {
  productId: string;
  productName: string;
  thumbnailUrl?: string;
  unitPrice: number;
  quantity: number;
}

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './checkout.html'
})
export class CheckoutComponent implements OnInit {
  form: FormGroup;
  cartItems: CheckoutDisplayItem[] = [];
  totalAmount = 0;

  constructor(
    private fb: FormBuilder,
    private orderService: Orderservice,
    private router: Router,
    private cartService: CartHttpService,
    private http: HttpClient
  ) {
    this.form = this.fb.group({
      fullName: ['', Validators.required],
      phone: ['', Validators.required],
      addressLine1: ['', Validators.required],
      addressLine2: [''],
      city: ['', Validators.required],
      state: ['', Validators.required],
      pincode: ['', Validators.required]
    });
  }

  ngOnInit() {
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