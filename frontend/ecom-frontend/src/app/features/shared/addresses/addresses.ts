import { Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MenuModule } from 'primeng/menu';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MenuItem } from 'primeng/api';
import { AddressService } from '../services/address-service';
import { AddAddressCommand, AddressDto } from '../models/address-model';
import { DropdownModule } from 'primeng/dropdown';


@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    ButtonModule,
    MenuModule,
    ToastModule,
    ConfirmDialogModule,
    DropdownModule
  ],
  templateUrl: './addresses.html',
  styleUrl: './addresses.css',
  providers: [MessageService, ConfirmationService]
})
export class Addresses implements OnInit {
  readonly addressTypes = [
    { label: 'Home', value: 'Home' },
    { label: 'Work', value: 'Work' },
    { label: 'Other', value: 'Other' }
  ];
  addresses: AddressDto[] = [];
  loading = true;
  error?: string;
  addAddressCommand: AddAddressCommand = this.getEmptyForm();
  submitting = false;
  isEditing = false;
  showForm = false;

  constructor(
    private addressService: AddressService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadAddresses();
  }

  private getEmptyForm(): AddAddressCommand {
    return {
      fullName: '',
      phone: '',
      addressLine1: '',
      addressLine2: '',
      city: '',
      state: '',
      pincode: '',
      country: '',
      addressType: 'Home',
      isDefault: false
    };
  }

  loadAddresses() {
    this.loading = true;
    this.addressService.getMyAddresses().subscribe({
      next: (addresses) => {
        this.addresses = addresses;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load addresses.';
        this.loading = false;
      }
    });
  }

  openAddForm() {
    this.isEditing = false;
    this.addAddressCommand = this.getEmptyForm();
    this.showForm = true;
  }

  openEditForm(address: AddressDto) {
    this.isEditing = true;
    this.addAddressCommand = { ...address };
    this.showForm = true;
  }

  cancelForm(form: NgForm) {
    this.showForm = false;
    form.resetForm();
    this.addAddressCommand = this.getEmptyForm();
  }

  saveAddress(form: NgForm) {
    if (form.invalid) return;

    this.submitting = true;
    const request = this.isEditing
      ? this.addressService.updateMyAddress((this.addAddressCommand as any).id, this.addAddressCommand)
      : this.addressService.addMyAddress(this.addAddressCommand);

    request.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: this.isEditing ? 'Address updated' : 'Address added'
        });
        this.loadAddresses();
        this.showForm = false;
        form.resetForm();
        this.submitting = false;
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Operation failed' });
        this.submitting = false;
      }
    });
  }

  menuItemsCache = new Map<string, MenuItem[]>();

  getMenuItems(addr: AddressDto): MenuItem[] {
    if (!this.menuItemsCache.has(addr.id)) {
      this.menuItemsCache.set(addr.id, [
        {
          label: 'Edit',
          icon: 'pi pi-pencil',
          command: () => this.openEditForm(addr)
        },
        {
          label: 'Delete',
          icon: 'pi pi-trash',
          command: () => this.deleteAddress(addr.id)
        }
      ]);
    }
    return this.menuItemsCache.get(addr.id)!;
  }

  deleteAddress(id: string) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this address?',
      header: 'Confirm Delete',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.addressService.deleteMyAddress(id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', detail: 'Address deleted' });
            this.loadAddresses();
          },
          error: () => this.messageService.add({ severity: 'error', detail: 'Failed to delete' })
        });
      }
    });
  }
}