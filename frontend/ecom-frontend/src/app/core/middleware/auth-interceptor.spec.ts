import { TestBed } from '@angular/core/testing';
import { HttpInterceptorFn } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthInterceptor } from './auth-interceptor';

describe('AuthInterceptor', () => {
  const interceptor: HttpInterceptorFn = (req, next) => 
    TestBed.runInInjectionContext(() => AuthInterceptor(req, next));

  TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: []
  });

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(interceptor).toBeTruthy();
  });

  it('should be created with provided service', () => {
    expect(interceptor).toBeTruthy();
  });
});
