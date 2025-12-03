import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { OrderFailedComponent } from './order-failed';

describe('OrderFailedComponent', () => {
  let component: OrderFailedComponent;
  let fixture: ComponentFixture<OrderFailedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderFailedComponent, HttpClientTestingModule],
      providers: []
    })
      .compileComponents();

    fixture = TestBed.createComponent(OrderFailedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
