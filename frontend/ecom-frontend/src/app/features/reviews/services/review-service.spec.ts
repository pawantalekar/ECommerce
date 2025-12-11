import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ReviewService } from './review-service';
import { environment } from '../../../../environments/environment';
import { Review } from '../models/review.model';
import { CreateReviewRequest } from '../models/create-review-request.model';
import { UpdateReviewRequest } from '../models/update-review-request.model';

describe('ReviewService', () => {
  let service: ReviewService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBaseUrl}/reviews`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ReviewService]
    });
    service = TestBed.inject(ReviewService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get reviews for a product', () => {
    const productId = 'product123';
    const mockReviews: Review[] = [
      { id: '1', productId, userId: 'u1', rating: 5 } as Review,
      { id: '2', productId, userId: 'u2', rating: 4 } as Review
    ];

    service.getReviews(productId).subscribe(reviews => {
      expect(reviews).toEqual(mockReviews);
      expect(reviews.length).toBe(2);
    });

    const req = httpMock.expectOne(`${baseUrl}/product/${productId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockReviews);
  });

  it('should check if user can review a product', () => {
    const productId = 'product123';

    service.canReview(productId).subscribe(canReview => {
      expect(canReview).toBe(true);
    });

    const req = httpMock.expectOne(`${baseUrl}/can-review/${productId}`);
    expect(req.request.method).toBe('GET');
    req.flush(true);
  });

  it('should get my review for a product', () => {
    const productId = 'product123';
    const mockReview: Review = {
      id: 'review1',
      productId,
      userId: 'u1',
      rating: 5
    } as Review;

    service.getMyReview(productId).subscribe(review => {
      expect(review).toEqual(mockReview);
    });

    const req = httpMock.expectOne(`${baseUrl}/my/${productId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockReview);
  });

  it('should return null when user has no review', () => {
    const productId = 'product123';

    service.getMyReview(productId).subscribe(review => {
      expect(review).toBeNull();
    });

    const req = httpMock.expectOne(`${baseUrl}/my/${productId}`);
    expect(req.request.method).toBe('GET');
    req.flush(null);
  });

  it('should create a review', () => {
    const createRequest: CreateReviewRequest = {
      productId: 'product123',
      rating: 5,
      comment: 'Great product'
    } as CreateReviewRequest;
    const mockReview: Review = {
      id: 'review1',
      ...createRequest
    } as Review;

    service.createReview(createRequest).subscribe(review => {
      expect(review).toEqual(mockReview);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(mockReview);
  });

  it('should update a review', () => {
    const reviewId = 'review123';
    const updateRequest: UpdateReviewRequest = {
      rating: 4,
      comment: 'Updated comment'
    } as UpdateReviewRequest;
    const mockReview: Review = {
      id: reviewId,
      ...updateRequest
    } as Review;

    service.updateReview(reviewId, updateRequest).subscribe(review => {
      expect(review).toEqual(mockReview);
    });

    const req = httpMock.expectOne(`${baseUrl}/${reviewId}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updateRequest);
    req.flush(mockReview);
  });

  it('should delete a review', () => {
    const reviewId = 'review123';

    service.deleteReview(reviewId).subscribe(response => {
      expect(response).toBeNull();
    });

    const req = httpMock.expectOne(`${baseUrl}/${reviewId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
