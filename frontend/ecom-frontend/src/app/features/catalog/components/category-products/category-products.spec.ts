import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { CategoryProducts } from './category-products';

describe('CategoryProducts', () => {
  let component: CategoryProducts;
  let fixture: ComponentFixture<CategoryProducts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryProducts, HttpClientTestingModule],
      providers: [provideRouter([])]
    })
      .compileComponents();

    fixture = TestBed.createComponent(CategoryProducts);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
