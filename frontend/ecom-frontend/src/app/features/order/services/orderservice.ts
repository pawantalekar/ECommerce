import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CheckoutItem, Order, ShippingAddress } from '../models/order';
import { InitiatePaymentResponse } from '../models/payment';

@Injectable({
  providedIn: 'root'
})
export class Orderservice {
  private apiUrl = `${environment.apiBaseUrl}/payments`;

  constructor(private http: HttpClient) { }

  createOrder(items: CheckoutItem[], shipping: ShippingAddress): Observable<InitiatePaymentResponse> {
    return this.http.post<InitiatePaymentResponse>(`${this.apiUrl}/create`, { items, shippingAddress: shipping });
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${environment.apiBaseUrl}/orders/history`);
  }

  getOrderById(id: string): Observable<Order> {
    return this.http.get<Order>(`${environment.apiBaseUrl}/orders/${id}`);
  }
}