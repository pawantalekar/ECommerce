import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Table, TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { DropdownModule } from 'primeng/dropdown';
import { TooltipModule } from 'primeng/tooltip';
import { AdminApprovalService } from './services/admin-approval';
import { PendingReview } from './models/pending-review.model';
import { SellerRequest } from './models/seller-request.model';
import { AdminOrderService } from '../admin-order-management.component/services/admin-order.service';
import { AdminOrder } from '../admin-order-management.component/models/admin-order.model';
import { FormsModule } from '@angular/forms';
import { AdminOrderManagementComponent } from '../admin-order-management.component/admin-order-management.component';

@Component({
  selector: 'app-approvals',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    DropdownModule,
    TooltipModule,
    FormsModule,
    AdminOrderManagementComponent
  ],
  templateUrl: './approvals.html',
  styleUrls: ['./approvals.css']
})
export class ApprovalsComponent implements OnInit {
  @ViewChild('reviewTable') reviewTable!: Table;
  @ViewChild('requestTable') requestTable!: Table;

  pendingReviews: PendingReview[] = [];
  pendingRequests: SellerRequest[] = [];
  activeTab = "reviews";
  loading = true;

  private service = inject(AdminApprovalService);
  private router = inject(Router);

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;

    this.service.getPendingReviews().subscribe({
      next: (data) => {
        this.pendingReviews = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });

    this.service.getPendingSellerRequests().subscribe({
      next: (data) => this.pendingRequests = data
    });
  }

  approveReview(id: string) {
    this.service.approveReview(id).subscribe(() => this.loadData());
  }

  rejectReview(id: string) {
    this.service.rejectReview(id).subscribe(() => this.loadData());
  }

  approveSeller(id: string) {
    this.service.approveSellerRequest(id).subscribe(() => this.loadData());
  }

  rejectSeller(id: string) {
    this.service.rejectSellerRequest(id).subscribe(() => this.loadData());
  }
} 