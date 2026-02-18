
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Product, AddProductPayload, UpdateProductPayload, ProductForEdit, Brand, Category } from '../models/product'; // Import
import { environment } from '../../../../environments/environment';

export type { AddProductPayload };
export interface ProductResult {
  id: string;
  name: string;
  slug: string;
  shortDescription?: string;
  price: number;
  imageUrls: string[];
  tags: string[];
  isActive: boolean;
  isFeatured: boolean;
  sku: string;
  stockQuantity: number;
  categoryName: string;
  brandName?: string;
}

interface ToggleResponse {
  isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class Catalog {
  private http = inject(HttpClient);

  private readonly api = `${environment.apiBaseUrl}/CatalogService`;

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.api}/products`);
  }

  getBySlug(slug: string): Observable<Product> {
    return this.http.get<Product>(`${this.api}/products/${slug}`);
  }

  uploadImage(file: File): Observable<{ url: string }> {
    const fd = new FormData();
    fd.append('file', file);
    return this.http.post<{ url: string }>(`${this.api}/upload`, fd);
  }

  addProduct(payload: AddProductPayload): Observable<any> {
    return this.http.post(`${this.api}/products`, payload);
  }
  getMyProducts(): Observable<ProductResult[]> {
    return this.http.get<ProductResult[]>(`${this.api}/my-products`);
  }

  toggleProductActive(productId: string): Observable<ToggleResponse> {
    return this.http.patch<ToggleResponse>(
      `${this.api}/my-products/${productId}/toggle-active`,
      {}
    );
  }
  updateProduct(productId: string, payload: UpdateProductPayload): Observable<void> {
    return this.http.put<void>(`${this.api}/my-products/${productId}`, payload);
  }
  GetProductForEdit(productId: string): Observable<ProductForEdit> {
    return this.http.get<ProductForEdit>(`${this.api}/my-products/${productId}`);
  }

  GetFeaturedProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.api}/featured-products`);
  }
  GetAllBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>(`${this.api}/brands`);
  }
  GetAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.api}/categories`);
  }
  getProductsByCategory(categoryId: string): Observable<Product[]> {
    return this.http.get<any[]>(`${this.api}/categories/${categoryId}/products`).pipe(
      map(products => products.map(p => ({
        ...p,
        imageUrls: Array.isArray(p.imageUrls) ? p.imageUrls : (Array.isArray(p.imageurls) ? p.imageurls : [])
      })))
    );
  }

}