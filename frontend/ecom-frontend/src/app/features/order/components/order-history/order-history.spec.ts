import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { OrderHistoryComponent } from './order-history';
import { Orderservice } from '../../services/orderservice';
import { Order } from '../../models/order';

describe('OrderHistoryComponent', () => {
  let component: OrderHistoryComponent;
  let fixture: ComponentFixture<OrderHistoryComponent>;
  let orderServiceSpy: jasmine.SpyObj<Orderservice>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    orderServiceSpy = jasmine.createSpyObj('Orderservice', ['getMyOrders']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [OrderHistoryComponent],
      providers: [
        { provide: Orderservice, useValue: orderServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    orderServiceSpy.getMyOrders.and.returnValue(of([]));
    fixture = TestBed.createComponent(OrderHistoryComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load orders on init', (done) => {
    const mockOrders: Order[] = [
      { id: 'order1', total: 100 } as unknown as Order,
      { id: 'order2', total: 200 } as unknown as Order
    ];
    orderServiceSpy.getMyOrders.and.returnValue(of(mockOrders));

    component.ngOnInit();

    setTimeout(() => {
      expect(orderServiceSpy.getMyOrders).toHaveBeenCalled();
      expect(component.orders).toEqual(mockOrders);
      expect(component.orders.length).toBe(2);
      done();
    }, 0);
  });

  it('should initialize with empty orders array', () => {
    expect(component.orders).toEqual([]);
  });

  it('should navigate to order detail when viewDetail is called', () => {
    const orderId = 'order123';

    component.viewDetail(orderId);

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/orders', orderId]);
  });
});
