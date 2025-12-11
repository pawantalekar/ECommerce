import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { provideRouter } from '@angular/router';
import { CartItemComponent } from './cart-item';
import { CartStore } from '../../signals/cart.store';
import { CartItemDto } from '../../models/cart';

describe('CartItemComponent', () => {
  let component: CartItemComponent;
  let fixture: ComponentFixture<CartItemComponent>;
  let cartStoreSpy: jasmine.SpyObj<CartStore>;

  beforeEach(async () => {
    cartStoreSpy = jasmine.createSpyObj<CartStore>(
      'CartStore',
      ['updateQuantity', 'removeFromCart'],
      { loading: signal(false) }
    );

    await TestBed.configureTestingModule({
      imports: [CartItemComponent],
      providers: [
        { provide: CartStore, useValue: cartStoreSpy },
        provideRouter([])
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CartItemComponent);
    component = fixture.componentInstance;
    component.item = { productId: 'p123' } as CartItemDto;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should not call updateQuantity when qty is less than 1', () => {
    component.updateQuantity(0);
    expect(cartStoreSpy.updateQuantity).not.toHaveBeenCalled();
  });

  it('should not call updateQuantity when quantitty is negative', () => {
    component.updateQuantity(-5);
    expect(cartStoreSpy.updateQuantity).not.toHaveBeenCalled();
  });

  it('should call updateQuantity when quantity is greater than 1', () => {
    component.updateQuantity(5);
    expect(cartStoreSpy.updateQuantity).toHaveBeenCalledOnceWith('p123', 5);
  });

  it('should call removeFromCart  method of cartStore  when remove is called', () => {
    component.remove();
    expect(cartStoreSpy.removeFromCart).toHaveBeenCalledOnceWith('p123');
  });

});