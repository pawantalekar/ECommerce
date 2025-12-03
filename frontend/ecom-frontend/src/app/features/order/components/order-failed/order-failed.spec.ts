import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderFailed } from './order-failed';

describe('OrderFailed', () => {
  let component: OrderFailed;
  let fixture: ComponentFixture<OrderFailed>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderFailed]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderFailed);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
