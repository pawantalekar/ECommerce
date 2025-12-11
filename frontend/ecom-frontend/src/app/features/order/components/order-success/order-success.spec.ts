import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { OrderSuccessComponent } from './order-success';
import { Router } from '@angular/router';


describe('OrderSuccessComponent', () => {
  let component: OrderSuccessComponent;
  let fixture: ComponentFixture<OrderSuccessComponent>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
     routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    await TestBed.configureTestingModule({
      imports: [OrderSuccessComponent, HttpClientTestingModule],
      providers: [ { provide: Router, useValue: routerSpy } ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(OrderSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  
  it('should redirect user to /orders when goToOrders is called', () => {
    component.goToOrders();
    expect(routerSpy.navigate).toHaveBeenCalledOnceWith(['/orders']);
  });

});
