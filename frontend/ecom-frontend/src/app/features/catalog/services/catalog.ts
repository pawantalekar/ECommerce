
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, AddProductPayload } from '../models/product'; // Import
import { environment } from '../../../../environments/environment';

export type { AddProductPayload };

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
}