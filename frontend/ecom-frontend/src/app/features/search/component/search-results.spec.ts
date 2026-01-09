import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SearchResults } from './search-results';
import { provideRouter } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ProductSearchService } from '../services/product-search.service';

describe('SearchResults', () => {
  let component: SearchResults;
  let fixture: ComponentFixture<SearchResults>;
  let httpMock: HttpTestingController;
  let activatedRoute: ActivatedRoute;
  let searchService: ProductSearchService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchResults, HttpClientTestingModule],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({ q: 'laptop' })
          }
        }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(SearchResults);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    activatedRoute = TestBed.inject(ActivatedRoute);
    searchService = TestBed.inject(ProductSearchService);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should perform search when query parameter is provided', (done) => {
      fixture.detectChanges();

      const req = httpMock.expectOne(`${environment.apiBaseUrl}/search?q=laptop`);
      expect(req.request.method).toBe('GET');
      expect(component.query).toBe('laptop');

      req.flush({ products: [{ id: '1', name: 'Laptop' }], totalCount: 1 });

      component.result$.subscribe(result => {
        if (result) {
          expect(result.products.length).toBe(1);
          expect(result.products[0]).toEqual(jasmine.objectContaining({ id: '1', name: 'Laptop' }));
          expect(result.totalCount).toBe(1);
          done();
        }
      });
    });

    it('should not search when query parameter is empty', (done) => {
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        imports: [SearchResults, HttpClientTestingModule],
        providers: [
          provideRouter([]),
          {
            provide: ActivatedRoute,
            useValue: { queryParams: of({ q: '' }) }
          }
        ]
      });
      fixture = TestBed.createComponent(SearchResults);
      component = fixture.componentInstance;
      searchService = TestBed.inject(ProductSearchService);

      fixture.detectChanges();

      httpMock.expectNone(`${environment.apiBaseUrl}/search?q=`);

      component.result$.subscribe(result => {
        if (result !== null) {
          expect(result.products).toEqual([]);
          expect(result.totalCount).toBe(0);
          done();
        }
      });
    });

    it('should trim whitespace from query parameter', () => {
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        imports: [SearchResults, HttpClientTestingModule],
        providers: [
          provideRouter([]),
          {
            provide: ActivatedRoute,
            useValue: { queryParams: of({ q: '  laptop  ' }) }
          }
        ]
      });
      fixture = TestBed.createComponent(SearchResults);
      component = fixture.componentInstance;
      const newHttpMock = TestBed.inject(HttpTestingController);

      fixture.detectChanges();

      const req = newHttpMock.expectOne(`${environment.apiBaseUrl}/search?q=laptop`);
      req.flush({ products: [], totalCount: 0 });
      expect(component.query).toBe('laptop');

      newHttpMock.verify();
    });

    it('should handle search errors gracefully', (done) => {
      fixture.detectChanges();

      const req = httpMock.expectOne(`${environment.apiBaseUrl}/search?q=laptop`);
      req.flush('Error', { status: 500, statusText: 'Server Error' });

      component.result$.subscribe(result => {
        if (result !== null) {
          expect(result.products).toEqual([]);
          expect(result.totalCount).toBe(0);
          done();
        }
      });
    });

    it('should handle missing query parameter', () => {
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        imports: [SearchResults, HttpClientTestingModule],
        providers: [
          provideRouter([]),
          {
            provide: ActivatedRoute,
            useValue: { queryParams: of({}) }
          }
        ]
      });
      fixture = TestBed.createComponent(SearchResults);
      component = fixture.componentInstance;

      fixture.detectChanges();

      httpMock.expectNone(`${environment.apiBaseUrl}/search`);
      expect(component.query).toBe('');
    });
  });
});
