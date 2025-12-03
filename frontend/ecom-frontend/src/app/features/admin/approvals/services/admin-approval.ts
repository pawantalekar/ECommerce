import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { PendingReview } from '../models/pending-review.model';
import { SellerRequest } from '../models/seller-request.model';

@Injectable({
  providedIn: 'root'
})
export class AdminApprovalService {
  private baseUrl = `${environment.apiBaseUrl}/admin`;

  constructor(private http: HttpClient) { }

  getPendingReviews() {
    return this.http.get<PendingReview[]>(`${this.baseUrl}/reviews/pending`);
  }

  approveReview(id: string) {
    return this.http.post(`${this.baseUrl}/reviews/${id}/approve`, {});
  }

  rejectReview(id: string) {
    return this.http.post(`${this.baseUrl}/reviews/${id}/reject`, {});
  }

  getPendingSellerRequests() {
    return this.http.get<SellerRequest[]>(`${this.baseUrl}/seller-requests/pending`);
  }

  approveSellerRequest(id: string) {
    return this.http.post(`${this.baseUrl}/seller-requests/${id}/approve`, {});
  }

  rejectSellerRequest(id: string) {
    return this.http.post(`${this.baseUrl}/seller-requests/${id}/reject`, {});
  }
}