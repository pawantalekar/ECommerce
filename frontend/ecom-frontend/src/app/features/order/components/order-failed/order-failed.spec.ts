import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { OrderFailedComponent } from './order-failed';

describe('OrderFailedComponent', () => {
  let component: OrderFailedComponent;
  let fixture: ComponentFixture<OrderFailedComponent>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    await TestBed.configureTestingModule({
      imports: [OrderFailedComponent],
      providers: [{ provide: Router, useValue: routerSpy }]
    })
      .compileComponents();

    fixture = TestBed.createComponent(OrderFailedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to /orders when goToOrders is called', () => {
    component.goToOrders();
    expect(routerSpy.navigate).toHaveBeenCalledOnceWith(['/orders']);
  });
});
