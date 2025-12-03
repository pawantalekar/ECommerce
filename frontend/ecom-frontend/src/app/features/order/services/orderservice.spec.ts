import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Orderservice } from './orderservice';

describe('Orderservice', () => {
  let service: Orderservice;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: []
    });
    service = TestBed.inject(Orderservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
