import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface ProductSearchResultDto {
    slug: string;
    name: string;
    shortDescription: string;
    price: number;
    imageUrl: string;
    imageUrls: string[];
    categoryId: string | null;
    brandName: string;
    isFeatured: boolean;
    tags: string[];
    id: string;
    description: string | null;
    sku: string;
    stockQuantity: number;
    categoryName: string;
    isActive: boolean;
}

export interface SearchProductsResult {
    products: ProductSearchResultDto[];
    totalCount: number;
}

@Injectable({ providedIn: 'root' })
export class ProductSearchService {
    private apiUrl = `${environment.apiBaseUrl}/search`;
    private resultSubject = new BehaviorSubject<SearchProductsResult | null>(null);
    result$ = this.resultSubject.asObservable();

    constructor(private http: HttpClient) { }

    search(query: string, page = 1): void {
        if (!query?.trim()) {
            this.resultSubject.next({ products: [], totalCount: 0 });
            return;
        }

        const params: any = { q: query.trim() };
        if (page > 1) params.page = page;

        this.http.get<SearchProductsResult>(this.apiUrl, { params })
            .subscribe({
                next: (res) => {
                    const mappedProducts = res.products.map(p => ({
                        ...p,
                        imageUrls: p.imageUrls || (p.imageUrl ? [p.imageUrl] : [])
                    }));
                    this.resultSubject.next({ ...res, products: mappedProducts });
                },
                error: (err) => {
                    console.error('Search error:', err);
                    this.resultSubject.next({ products: [], totalCount: 0 });
                }
            });
    }
}