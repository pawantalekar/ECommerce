import { Component, OnInit, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSelectModule } from 'primeng/multiselect';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { AdminOrderService } from './services/admin-order.service';
import { AdminOrder, OrderFilters, PagedOrdersResponse, ORDER_STATUSES } from './models/admin-order.model';
import { TableModule, Table } from "primeng/table";
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-admin-order-management',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MultiSelectModule,
    DropdownModule,
    ButtonModule,
    TableModule,
    CalendarModule,
    DialogModule,
    InputTextareaModule,
    ToastModule,
    TooltipModule
  ],
  providers: [MessageService],
  templateUrl: './admin-order-management.component.html',
  styleUrl: './admin-order-management.component.css',
})
export class AdminOrderManagementComponent implements OnInit {
  @ViewChild('dt') dt!: Table;

  private orderService = inject(AdminOrderService);
  private messageService = inject(MessageService);

  orders: AdminOrder[] = [];
  loading = true;
  totalRecords = 0;
  first = 0;

  selectedOrderStatuses: string[] = [];
  selectedPaymentStatus: string[] = [];
  searchTerm = '';
  fromDate: Date | null = null;
  toDate: Date | null = null;
  currentPage = 1;
  pageSize = 20;

  // Status update dialog
  showStatusDialog = false;
  selectedOrder: AdminOrder | null = null;
  newStatus = '';
  statusNotes = '';
  updatingStatus = false;

  // Available statuses for dropdown
  availableStatuses = ORDER_STATUSES.map(status => ({
    label: this.formatStatusLabel(status),
    value: status
  }));

  orderStatuses = [
    { label: 'Confirmed', value: 'Confirmed' },
    { label: 'Pending', value: 'Pending' },
    { label: 'Processing', value: 'Processing' },
    { label: 'Shipped', value: 'Shipped' },
    { label: 'Out For Delivery', value: 'OutForDelivery' },
    { label: 'Delivered', value: 'Delivered' },
    { label: 'Cancelled', value: 'Cancelled' },
    { label: 'Failed', value: 'Failed' }
  ];

  paymentStatuses = [
    { label: 'Pending', value: 'Pending' },
    { label: 'Paid', value: 'Paid' },
    { label: 'Failed', value: 'Failed' }
  ];


  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.loading = true;

    const filters: OrderFilters = {
      orderStatus: this.selectedOrderStatuses?.length > 0 ? this.selectedOrderStatuses : undefined,
      paymentStatus: this.selectedPaymentStatus?.length > 0 ? this.selectedPaymentStatus : undefined,
      searchTerm: this.searchTerm || undefined,
      fromDate: this.fromDate || undefined,
      toDate: this.toDate || undefined,
      pageNumber: this.currentPage,
      pageSize: this.pageSize
    };

    this.orderService.getAllOrders(filters).subscribe({
      next: (response: PagedOrdersResponse) => {
        this.orders = response.orders;
        this.totalRecords = response.total;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading orders:', error);
        this.loading = false;
      }
    });
  }

  onFilterChange() {
    this.first = 0;
    this.currentPage = 1;
    this.loadOrders();
  }

  onPageChange(event: any) {
    this.first = event.first;
    this.currentPage = event.page + 1;
    this.pageSize = event.rows;
    this.loadOrders();
  }

  onSearch() {
    this.first = 0;
    this.currentPage = 1;
    this.loadOrders();
  }

  getOrderStatusClass(status: string): string {
    const classMap: Record<string, string> = {
      'Confirmed': 'badge-success',
      'Pending': 'badge-warning',
      'Shipped': 'badge-info',
      'Delivered': 'badge-success',
      'Cancelled': 'badge-danger',
      'Failed': 'badge-danger'
    };
    return classMap[status] || 'badge-secondary';
  }

  getPaymentStatusClass(status: string): string {
    const classMap: Record<string, string> = {
      'Paid': 'badge-success',
      'Pending': 'badge-warning',
      'Failed': 'badge-danger'
    };
    return classMap[status] || 'badge-secondary';
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatAmount(amount: number): string {
    return '₹' + amount.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  formatStatusLabel(status: string): string {
    return status.replace(/([A-Z])/g, ' $1').trim();
  }

  openStatusDialog(order: AdminOrder) {
    this.selectedOrder = order;
    this.newStatus = order.orderStatus;
    this.statusNotes = '';
    this.showStatusDialog = true;
  }

  closeStatusDialog() {
    this.showStatusDialog = false;
    this.selectedOrder = null;
    this.newStatus = '';
    this.statusNotes = '';
  }

  updateOrderStatus() {
    if (!this.selectedOrder || !this.newStatus) return;

    if (this.newStatus === this.selectedOrder.orderStatus) {
      this.messageService.add({
        severity: 'warn',
        summary: 'No Change',
        detail: 'Order is already in this status'
      });
      return;
    }

    this.updatingStatus = true;

    this.orderService.updateOrderStatus(
      this.selectedOrder.orderNumber,
      this.newStatus,
      this.statusNotes || undefined
    ).subscribe({
      next: (response) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Status Updated',
          detail: `Order ${response.orderNumber} updated to ${response.status}`
        });
        this.updatingStatus = false;
        this.closeStatusDialog();
        this.loadOrders();
      },
      error: (error) => {
        console.error('Error updating status:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Update Failed',
          detail: error.error?.message || 'Failed to update order status'
        });
        this.updatingStatus = false;
      }
    });
  }
}
