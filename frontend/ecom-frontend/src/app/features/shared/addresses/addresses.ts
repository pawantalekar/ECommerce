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
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputTextareaModule } from 'primeng/inputtextarea';


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
    DropdownModule,
    CheckboxModule,
    InputTextModule,
    RadioButtonModule,
    InputTextareaModule
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

  // UI State
  showAddForm = false;
  editingAddressId: string | null = null; // Track which address is being edited

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

  toggleAddForm() {
    if (this.showAddForm) {
      this.cancelAddForm();
    } else {
      this.showAddForm = true;
      this.editingAddressId = null; // Close any open edits
      this.addAddressCommand = this.getEmptyForm();
    }
  }

  cancelAddForm() {
    this.showAddForm = false;
    this.addAddressCommand = this.getEmptyForm();
  }

  // Edit Logic
  startEditing(address: AddressDto) {
    this.editingAddressId = address.id;
    this.showAddForm = false; // Close add form
    // create a copy for the form
    this.addAddressCommand = { ...address };
  }

  cancelEdit() {
    this.editingAddressId = null;
    this.addAddressCommand = this.getEmptyForm();
  }

  saveAddress(form: NgForm) {
    if (form.invalid) return;

    this.submitting = true;
    const isEdit = !!this.editingAddressId;

    // For edit, we use editingAddressId, for add we use addAddressCommand
    const request = isEdit
      ? this.addressService.updateMyAddress(this.editingAddressId!, this.addAddressCommand)
      : this.addressService.addMyAddress(this.addAddressCommand);

    request.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: isEdit ? 'Address updated' : 'Address added'
        });
        this.loadAddresses();

        // Reset state
        this.showAddForm = false;
        this.editingAddressId = null;
        this.addAddressCommand = this.getEmptyForm();
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
    // Always return fresh items to ensure closure captures current addr
    return [
      {
        label: 'Edit',
        command: () => this.startEditing(addr)
      },
      {
        label: 'Delete',
        command: () => this.deleteAddress(addr.id)
      }
    ];
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