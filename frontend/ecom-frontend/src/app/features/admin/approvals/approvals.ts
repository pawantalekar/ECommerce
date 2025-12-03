import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminApprovalService } from './services/admin-approval';
import { PendingReview } from './models/pending-review.model';
import { SellerRequest } from './models/seller-request.model';

@Component({
  selector: 'app-approvals',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './approvals.html',
  // styleUrls: ['./approvals.scss']
})
export class ApprovalsComponent implements OnInit {
  pendingReviews: PendingReview[] = [];
  pendingRequests: SellerRequest[] = [];
  activeTab = 'reviews';
  loading = true;

  constructor(private service: AdminApprovalService) { }

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
      next: (data) => this.pendingRequests = data,
      error: () => { }
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