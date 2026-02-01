import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { PagedOrdersResponse, OrderFilters, UpdateOrderStatusResponse, OrderStatusHistory } from '../models/admin-order.model';

@Injectable({
  providedIn: 'root'
})
export class AdminOrderService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBaseUrl}/orders/admin/all`;

  getAllOrders(filters: OrderFilters = {}): Observable<PagedOrdersResponse> {
    let params = new HttpParams();

    if (filters.orderStatus && filters.orderStatus.length > 0) {
      filters.orderStatus.forEach(status => {
        params = params.append('orderStatus', status);
      });
    }
    if (filters.paymentStatus && filters.paymentStatus.length > 0) {
      filters.paymentStatus.forEach(status => {
        params = params.append('paymentStatus', status);
      });
    }
    if (filters.fromDate) {
      params = params.set('fromDate', filters.fromDate.toISOString());
    }
    if (filters.toDate) {
      params = params.set('toDate', filters.toDate.toISOString());
    }
    if (filters.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }
    if (filters.pageNumber) {
      params = params.set('pageNumber', filters.pageNumber.toString());
    }
    if (filters.pageSize) {
      params = params.set('pageSize', filters.pageSize.toString());
    }

    return this.http.get<PagedOrdersResponse>(this.baseUrl, { params });
  }

  updateOrderStatus(orderNumber: string, status: string, notes?: string): Observable<UpdateOrderStatusResponse> {
    return this.http.patch<UpdateOrderStatusResponse>(
      `${environment.apiBaseUrl}/orders/admin/orders/${orderNumber}/status`,
      { status, notes }
    );
  }

  getOrderStatusHistory(orderNumber: string): Observable<OrderStatusHistory[]> {
    return this.http.get<OrderStatusHistory[]>(
      `${environment.apiBaseUrl}/orders/orders/${orderNumber}/status-history`
    );
  }
}
