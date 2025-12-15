import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { ProductCard } from '../product-card/product-card';
import { Product } from '../../models/product';

interface ProductResult {
  id: string;
  name: string;
  slug: string;
  shortDescription?: string;
  description?: string;
  price: number;
  sku: string;
  stockQuantity: number;
  categoryName: string;
  brandName?: string;
  isActive: boolean;
  isFeatured: boolean;
  imageUrls: string[];
  tags: string[];
}

@Component({
  selector: 'app-my-products',
  standalone: true,
  imports: [CommonModule, ProductCard],
  templateUrl: './my-products.html',
  styleUrls: ['./my-products.css']
})
export class MyProducts implements OnInit {
  products: ProductResult[] = [];
  loading = true;
  error: string | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadMyProducts();
  }
  asProduct(productResult: ProductResult): Product {
    return {
      ...productResult,
      shortDescription: productResult.shortDescription ?? null
    } as Product;
  }
  loadMyProducts(): void {
    this.loading = true;
    this.error = null;

    this.http.get<ProductResult[]>(`${environment.apiBaseUrl}/CatalogService/my-products`)
      .subscribe({
        next: (data) => {
          this.products = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load your products. Please try again later.';
          this.loading = false;
          console.error(err);
        }
      });
  }
}