import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { WriteReviewPageComponent } from './write-review-page';
import { provideRouter, Router } from '@angular/router';
import { ReviewService } from '../../services/review-service';
import { of, throwError } from 'rxjs';
import { Review } from '../../models/review.model';

describe('WriteReviewPageComponent', () => {
  let component: WriteReviewPageComponent;
  let fixture: ComponentFixture<WriteReviewPageComponent>;
  let reviewService: jasmine.SpyObj<ReviewService>;
  let router: Router;

  const mockReview: Review = {
    id: 'review-1',
    productId: 'prod-1',
    userId: 'user-1',
    rating: 4,
    comment: 'Great product',
    createdAt: '2025-01-01',
    updatedAt: null,
    userName: 'Test User'
  };

  beforeEach(async () => {
    const reviewServiceSpy = jasmine.createSpyObj('ReviewService', [
      'getMyReview',
      'createReview',
      'updateReview',
      'deleteReview'
    ]);

    // Mock history.state
    Object.defineProperty(window.history, 'state', {
      writable: true,
      value: { productId: 'prod-1' }
    });

    await TestBed.configureTestingModule({
      imports: [WriteReviewPageComponent, HttpClientTestingModule],
      providers: [
        provideRouter([]),
        { provide: ReviewService, useValue: reviewServiceSpy }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(WriteReviewPageComponent);
    component = fixture.componentInstance;
    reviewService = TestBed.inject(ReviewService) as jasmine.SpyObj<ReviewService>;
    router = TestBed.inject(Router);
    reviewService.getMyReview.and.returnValue(of(null));

  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should navigate to home if no productId is provided', () => {
      spyOn(window, 'alert');
      spyOn(router, 'navigate');

      // Override history.state to have no productId
      Object.defineProperty(window.history, 'state', {
        writable: true,
        value: {}
      });

      component.productId = '';
      component.ngOnInit();

      expect(window.alert).toHaveBeenCalledWith('No product selected for review');
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should load existing review and set edit mode', () => {
      component.productId = 'prod-1';
      reviewService.getMyReview.and.returnValue(of(mockReview));

      component.ngOnInit();

      expect(reviewService.getMyReview).toHaveBeenCalledWith('prod-1');
      expect(component.isEdit).toBe(true);
      expect(component.review).toEqual(mockReview);
      expect(component.form.value.rating).toBe(4);
      expect(component.form.value.comment).toBe('Great product');
    });

    it('should not set edit mode when no existing review', () => {
      component.productId = 'prod-1';
      reviewService.getMyReview.and.returnValue(of(null));

      component.ngOnInit();

      expect(component.isEdit).toBe(false);
      expect(component.review).toBeNull();
    });
  });

  describe('submit', () => {
    beforeEach(() => {
      component.productId = 'prod-1';
    });

    it('should create new review when not in edit mode', () => {
      spyOn(router, 'navigate');
      component.isEdit = false;
      component.form.patchValue({ rating: 5, comment: 'Excellent' });
      reviewService.createReview.and.returnValue(of({} as Review));

      component.submit();

      expect(reviewService.createReview).toHaveBeenCalledWith({
        productId: 'prod-1',
        rating: 5,
        comment: 'Excellent'
      });
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should update existing  review when in edit mode', () => {
      spyOn(router, 'navigate');
      component.isEdit = true;
      component.review = mockReview;
      component.form.patchValue({ rating: 3, comment: 'Updated' });
      reviewService.updateReview.and.returnValue(of({} as Review));

      component.submit();

      expect(reviewService.updateReview).toHaveBeenCalledWith('review-1', jasmine.objectContaining({
        rating: 3,
        comment: 'Updated'
      }));
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should not submit when form is invalid', () => {
      component.form.patchValue({ rating: 0, comment: '' });

      component.submit();

      expect(reviewService.createReview).not.toHaveBeenCalled();
      expect(reviewService.updateReview).not.toHaveBeenCalled();
    });

    it('should show error alert on submission failure', () => {
      spyOn(window, 'alert');
      component.form.patchValue({ rating: 5, comment: 'Test' });
      reviewService.createReview.and.returnValue(throwError(() => ({ error: { title: 'Error occurred' } })));

      component.submit();

      expect(window.alert).toHaveBeenCalledWith('Failed to save review: Error occurred');
    });
  });

  describe('delete', () => {
    it('should delete review when confirmed', () => {
      spyOn(window, 'confirm').and.returnValue(true);
      spyOn(router, 'navigate');
      component.review = mockReview;
      reviewService.deleteReview.and.returnValue(of(void 0));

      component.delete();

      expect(reviewService.deleteReview).toHaveBeenCalledWith('review-1');
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should not delete review when cancelled', () => {
      spyOn(window, 'confirm').and.returnValue(false);
      component.review = mockReview;

      component.delete();

      expect(reviewService.deleteReview).not.toHaveBeenCalled();
    });

    it('should not delete when no review exists', () => {
      component.review = null;

      component.delete();

      expect(reviewService.deleteReview).not.toHaveBeenCalled();
    });
  });

  describe('cancel', () => {
    it('should navigate back in history', () => {
      spyOn(window.history, 'back');

      component.cancel();

      expect(window.history.back).toHaveBeenCalled();
    });
  });

  describe('stars', () => {
    it('should return array of 1 to 5', () => {
      expect(component.stars).toEqual([1, 2, 3, 4, 5]);
    });
  });
});
