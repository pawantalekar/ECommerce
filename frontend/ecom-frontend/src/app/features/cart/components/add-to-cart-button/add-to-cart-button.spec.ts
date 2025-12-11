import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { signal } from '@angular/core';
import { AddToCartButtonComponent } from './add-to-cart-button';
import { CartStore } from '../../signals/cart.store';
import { AuthService } from '../../../../core/services/auth-service';

describe('AddToCartButton', () => {
  let component: AddToCartButtonComponent;
  let fixture: ComponentFixture<AddToCartButtonComponent>;
  let cartStoreSpy: jasmine.SpyObj<CartStore>;
  let routerSpy: jasmine.SpyObj<Router>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    cartStoreSpy = jasmine.createSpyObj('CartStore', ['addToCart'], {
      loading: signal(false)
    });
    routerSpy = jasmine.createSpyObj('Router', ['navigate'], {
      url: '/products/123'
    });
    authServiceSpy = jasmine.createSpyObj('AuthService', ['isLoggedIn']);

    await TestBed.configureTestingModule({
      imports: [AddToCartButtonComponent],
      providers: [
        { provide: CartStore, useValue: cartStoreSpy },
        { provide: Router, useValue: routerSpy },
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AddToCartButtonComponent);
    component = fixture.componentInstance;
    component.productId = 'product123';
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default quantity of 1', () => {
    expect(component.quantity).toBe(1);
  });

  it('should add to cart when user is logged in', () => {
    authServiceSpy.isLoggedIn.and.returnValue(true);

    component.addToCart();

    expect(authServiceSpy.isLoggedIn).toHaveBeenCalled();
    expect(cartStoreSpy.addToCart).toHaveBeenCalledWith('product123', 1);
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  });

  it('should navigate to login when user is not logged in', () => {
    authServiceSpy.isLoggedIn.and.returnValue(false);

    component.addToCart();

    expect(authServiceSpy.isLoggedIn).toHaveBeenCalled();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login'], {
      queryParams: { returnUrl: '/products/123' }
    });
    expect(cartStoreSpy.addToCart).not.toHaveBeenCalled();
  });

  it('should add to cart with custom quantity', () => {
    authServiceSpy.isLoggedIn.and.returnValue(true);
    component.quantity = 5;

    component.addToCart();

    expect(cartStoreSpy.addToCart).toHaveBeenCalledWith('product123', 5);
  });

  it('should convert productId to string when it is an object', () => {
    authServiceSpy.isLoggedIn.and.returnValue(true);
    component.productId = { toString: () => 'converted123' };

    component.addToCart();

    expect(cartStoreSpy.addToCart).toHaveBeenCalledWith('converted123', 1);
  });

  it('should use productId as string when it is already a string', () => {
    authServiceSpy.isLoggedIn.and.returnValue(true);
    component.productId = 'string123';

    component.addToCart();

    expect(cartStoreSpy.addToCart).toHaveBeenCalledWith('string123', 1);
  });
});
