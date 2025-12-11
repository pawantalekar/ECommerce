import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Orderservice } from './orderservice';
import { environment } from '../../../../environments/environment';
import { CheckoutItem, Order, ShippingAddress } from '../models/order';
import { InitiatePaymentResponse } from '../models/payment';

describe('Orderservice', () => {
  let service: Orderservice;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [Orderservice]
    });
    service = TestBed.inject(Orderservice);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create order', () => {
    const mockItems: CheckoutItem[] = [
      { productId: 'p1', quantity: 2, price: 100 } as CheckoutItem
    ];
    const mockShipping: ShippingAddress = {
      fullName: 'Test User',
      phone: '1234567890',
      addressLine1: '123 Main St',
      city: 'Test City',
      state: 'Test State',
      pincode: '12345'
    } as ShippingAddress;
    const mockResponse: InitiatePaymentResponse = {
      orderId: 'order123',
      paymentUrl: 'https://payment.com',
      razorpayOrderId: 'razorpay_order123',
      amount: 200,
      keyId: 'test_key_id'
    } as InitiatePaymentResponse;

    service.createOrder(mockItems, mockShipping).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/payments/create`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ items: mockItems, shippingAddress: mockShipping });
    req.flush(mockResponse);
  });

  it('should get my orders', () => {
    const mockOrders: Order[] = [
      { id: 'order1', total: 200 } as unknown as Order,
      { id: 'order2', total: 300 } as unknown as Order
    ];

    service.getMyOrders().subscribe(orders => {
      expect(orders).toEqual(mockOrders);
      expect(orders.length).toBe(2);
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/orders/history`);
    expect(req.request.method).toBe('GET');
    req.flush(mockOrders);
  });

  it('should get order by id', () => {
    const orderId = 'order123';
    const mockOrder: Order = {
      id: orderId,
      total: 500
    } as unknown as Order;

    service.getOrderById(orderId).subscribe(order => {
      expect(order).toEqual(mockOrder);
      expect(order.id).toBe(orderId);
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/orders/${orderId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockOrder);
  });
});
