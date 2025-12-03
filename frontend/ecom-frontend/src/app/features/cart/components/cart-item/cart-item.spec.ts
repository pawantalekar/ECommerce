import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CartItemComponent } from './cart-item';
import { provideRouter } from '@angular/router';

describe('CartItem', () => {
  let component: CartItemComponent;
  let fixture: ComponentFixture<CartItemComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartItemComponent, HttpClientTestingModule],
      providers: [provideRouter([])]
    })
      .compileComponents();

    fixture = TestBed.createComponent(CartItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
