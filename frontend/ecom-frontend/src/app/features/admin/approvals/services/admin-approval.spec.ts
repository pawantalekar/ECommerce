import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AdminApprovalService } from './admin-approval';

describe('AdminApprovalService', () => {
  let service: AdminApprovalService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: []
    });
    service = TestBed.inject(AdminApprovalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
