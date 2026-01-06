import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { CartPageComponent } from './cart-page';

describe('CartPage', () => {
  let component: CartPageComponent;
  let fixture: ComponentFixture<CartPageComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CartPageComponent, HttpClientTestingModule],
      providers: [provideRouter([])]
    })
      .compileComponents();

    fixture = TestBed.createComponent(CartPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
