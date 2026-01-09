import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ProductCard } from './product-card';
import { provideRouter } from '@angular/router';
import { Product } from '../../models/product';

describe('ProductCard', () => {
  let component: ProductCard;
  let fixture: ComponentFixture<ProductCard>;

  const mockProduct: Product = {
    id: 'prod-1',
    name: 'Test Product',
    slug: 'test-product',
    description: 'Test description',
    shortDescription: 'Test short description',
    price: 99.99,
    imageUrls: ['test.jpg'],
    categoryId: 'cat-1',
    categoryName: 'Test Category',
    brandName: 'Test Brand',
    isFeatured: false,
    tags: [],
    sku: 'SKU-001',
    stockQuantity: 10,
    isActive: true
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductCard, HttpClientTestingModule],
      providers: [provideRouter([])]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ProductCard);
    component = fixture.componentInstance;
    component.product = mockProduct;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
