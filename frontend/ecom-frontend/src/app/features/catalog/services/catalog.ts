
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, AddProductPayload, UpdateProductPayload, ProductForEdit } from '../models/product'; // Import
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
}