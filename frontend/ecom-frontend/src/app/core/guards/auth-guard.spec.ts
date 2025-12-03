import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthGuard } from './auth-guard';

describe('authGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => TestBed.inject(AuthGuard).canActivate());

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
