import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CartDto } from '../models/cart';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class CartHttpService {
    private apiUrl = `${environment.apiBaseUrl}/Cart`;

    constructor(private http: HttpClient) { }

    getCart(): Observable<CartDto> {
        return this.http.get<any>(this.apiUrl).pipe(map(res => res.cart ?? res));
    }

    addToCart(productId: string, quantity = 1): Observable<CartDto> {
        return this.http.post<any>(`${this.apiUrl}/items`, { productId, quantity })
            .pipe(map(res => res.cart ?? res));
    }

    updateQuantity(productId: string, quantity: number): Observable<CartDto> {
        return this.http.put<any>(`${this.apiUrl}/items/${productId}`, { productId, quantity })
            .pipe(map(res => res.cart ?? res));
    }

    removeFromCart(productId: string): Observable<CartDto> {
        return this.http.delete<any>(`${this.apiUrl}/items/${productId}`)
            .pipe(map(res => res.cart ?? res));
    }
}