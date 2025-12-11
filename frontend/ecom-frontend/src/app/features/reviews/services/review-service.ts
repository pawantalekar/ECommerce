import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review } from '../models/review.model';
import { CreateReviewRequest } from '../models/create-review-request.model';
import { UpdateReviewRequest } from '../models/update-review-request.model';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private http = inject(HttpClient);

  private api = `${environment.apiBaseUrl}/reviews`;

  getReviews(productId: string): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.api}/product/${productId}`);
  }

  canReview(productId: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.api}/can-review/${productId}`);
  }

  getMyReview(productId: string): Observable<Review | null> {
    return this.http.get<Review | null>(`${this.api}/my/${productId}`);
  }

  createReview(dto: CreateReviewRequest): Observable<Review> {
    return this.http.post<Review>(this.api, dto);
  }

  updateReview(id: string, dto: UpdateReviewRequest): Observable<Review> {
    return this.http.put<Review>(`${this.api}/${id}`, dto);
  }

  deleteReview(id: string): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}