import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AdminGuard } from './admin-guard';

describe('AdminGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => TestBed.inject(AdminGuard).canActivate());

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: []
    });
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
