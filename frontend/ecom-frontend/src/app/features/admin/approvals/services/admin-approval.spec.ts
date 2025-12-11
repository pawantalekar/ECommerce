import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AdminApprovalService } from './admin-approval';
import { environment } from '../../../../../environments/environment';
import { PendingReview } from '../models/pending-review.model';

describe('AdminApprovalService', () => {
  let service: AdminApprovalService;
  let httpMock: HttpTestingController;
  const baseUrl = `${environment.apiBaseUrl}/admin`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AdminApprovalService]
    });
    service = TestBed.inject(AdminApprovalService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

it('approve user review',()=>{

    const reviewId = 'rev123';
    service.approveReview(reviewId).subscribe(response=>{
      expect(response).toEqual({});
  });
  const req = httpMock.expectOne(`${baseUrl}/reviews/${reviewId}/approve`);
    expect(req.request.method).toBe('POST');
     req.flush({});
});

it('should reject user review', () => {

  const reviewId='p123';
  service.rejectReview(reviewId).subscribe(response=>{
    expect(response).toEqual({});
  });
  const req = httpMock.expectOne(`${baseUrl}/reviews/${reviewId}/reject`);
  expect(req.request.method).toBe('POST');
  req.flush({});  
});

  it('should approve seller request', () => {

    const requestId = 'p123';
    service.approveSellerRequest(requestId).subscribe(response => {
      expect(response).toEqual({});
    });
    const req = httpMock.expectOne(`${baseUrl}/seller-requests/${requestId}/approve`);
    expect(req.request.method).toBe('POST');
    req.flush({});

  });


  it('should reject seller request', () => {
    const requestId = 'synoptek123';
    service.rejectSellerRequest(requestId).subscribe(response => {
      expect(response).toEqual({});
    });
    const req = httpMock.expectOne(`${baseUrl}/seller-requests/${requestId}/reject`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('should fetch pending reviews successfully', () => {
    const mockReviews: PendingReview[] = [
      { id: '1', productId: 'p1', userId: 'u1', rating: 5, comment: 'Great product' } as PendingReview,
      { id: '2', productId: 'p2', userId: 'u2', rating: 4, comment: 'Good' } as PendingReview
    ];

    service.getPendingReviews().subscribe(reviews => {
      expect(reviews).toEqual(mockReviews);
      expect(reviews.length).toBe(2);
      expect(reviews[0].id).toBe('1');
      expect(reviews[1].rating).toBe(4);
    });

    const req = httpMock.expectOne(`${baseUrl}/reviews/pending`);
    expect(req.request.method).toBe('GET');
    req.flush(mockReviews);
  });
});
