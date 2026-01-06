import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject, throwError } from 'rxjs';
import { ApprovalsComponent } from './approvals';
import { AdminApprovalService } from './services/admin-approval';
import { PendingReview } from './models/pending-review.model';
import { SellerRequest } from './models/seller-request.model';

describe('ApprovalsComponent', () => {
  let component: ApprovalsComponent;
  let fixture: ComponentFixture<ApprovalsComponent>;
  let serviceSpy: jasmine.SpyObj<AdminApprovalService>;

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj('AdminApprovalService', [
      'getPendingReviews',
      'getPendingSellerRequests',
      'approveReview',
      'rejectReview',
      'approveSellerRequest',
      'rejectSellerRequest'
    ]);

    await TestBed.configureTestingModule({
      imports: [ApprovalsComponent],
      providers: [
        { provide: AdminApprovalService, useValue: serviceSpy }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    serviceSpy.getPendingReviews.and.returnValue(of([]));
    serviceSpy.getPendingSellerRequests.and.returnValue(of([]));

    fixture = TestBed.createComponent(ApprovalsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default values', () => {
    expect(component.pendingReviews).toEqual([]);
    expect(component.pendingRequests).toEqual([]);
    expect(component.activeTab).toBe('reviews');
    expect(component.loading).toBe(true);
  });

  describe('ngOnInit', () => {
    it('should call loadData on initialization', () => {
      spyOn(component, 'loadData');
      component.ngOnInit();
      expect(component.loadData).toHaveBeenCalled();
    });
  });

  describe('loadData', () => {
    it('should load pending reviews and requests successfully', (done) => {
      const mockReviews: PendingReview[] = [
        { id: '1', productId: 'p1', userId: 'u1' } as PendingReview
      ];
      const mockRequests: SellerRequest[] = [
        { id: '1', userId: 'u1' } as SellerRequest
      ];

      serviceSpy.getPendingReviews.and.returnValue(of(mockReviews));
      serviceSpy.getPendingSellerRequests.and.returnValue(of(mockRequests));

      component.loadData();

      setTimeout(() => {
        expect(component.pendingReviews).toEqual(mockReviews);
        expect(component.pendingRequests).toEqual(mockRequests);
        expect(component.loading).toBe(false);
        done();
      }, 0);
    });

    it('should set loading to true when loadData starts', () => {
      component.loading = false;
      const reviewsSubject = new Subject<PendingReview[]>();
      serviceSpy.getPendingReviews.and.returnValue(reviewsSubject.asObservable());

      component.loadData();
      expect(component.loading).toBe(true);

      reviewsSubject.next([]);
      reviewsSubject.complete();
    });

    it('should set loading to false on reviews error', (done) => {
      serviceSpy.getPendingReviews.and.returnValue(throwError(() => new Error()));
      serviceSpy.getPendingSellerRequests.and.returnValue(of([]));

      component.loadData();

      setTimeout(() => {
        expect(component.loading).toBe(false);
        done();
      }, 0);
    });

    it('should load seller requests', (done) => {
      const mockRequests: SellerRequest[] = [
        { id: '1', userId: 'u1' } as SellerRequest
      ];
      serviceSpy.getPendingReviews.and.returnValue(of([]));
      serviceSpy.getPendingSellerRequests.and.returnValue(of(mockRequests));

      component.loadData();

      setTimeout(() => {
        expect(serviceSpy.getPendingSellerRequests).toHaveBeenCalled();
        expect(component.pendingRequests).toEqual(mockRequests);
        done();
      }, 0);
    });
  });

  describe('approveReview', () => {
    it('should approve review and reload data', () => {
      serviceSpy.approveReview.and.returnValue(of({}));
      spyOn(component, 'loadData');

      component.approveReview('review123');

      expect(serviceSpy.approveReview).toHaveBeenCalledWith('review123');
      expect(component.loadData).toHaveBeenCalled();
    });
  });

  describe('rejectReview', () => {
    it('should reject review and reload data', () => {
      serviceSpy.rejectReview.and.returnValue(of({}));
      spyOn(component, 'loadData');

      component.rejectReview('review456');

      expect(serviceSpy.rejectReview).toHaveBeenCalledWith('review456');
      expect(component.loadData).toHaveBeenCalled();
    });
  });

  describe('approveSeller', () => {
    it('should approve seller request and reload data', () => {
      serviceSpy.approveSellerRequest.and.returnValue(of({}));
      spyOn(component, 'loadData');

      component.approveSeller('seller123');

      expect(serviceSpy.approveSellerRequest).toHaveBeenCalledWith('seller123');
      expect(component.loadData).toHaveBeenCalled();
    });
  });

  describe('rejectSeller', () => {
    it('should reject seller request and reload data', () => {
      serviceSpy.rejectSellerRequest.and.returnValue(of({}));
      spyOn(component, 'loadData');

      component.rejectSeller('seller456');

      expect(serviceSpy.rejectSellerRequest).toHaveBeenCalledWith('seller456');
      expect(component.loadData).toHaveBeenCalled();
    });
  });
});
