import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SearchResults } from './search-results';
import { provideRouter } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';

describe('SearchResults', () => {
  let component: SearchResults;
  let fixture: ComponentFixture<SearchResults>;
  let httpMock: HttpTestingController;
  let activatedRoute: ActivatedRoute;

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
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should perform search when query parameter is provided', () => {
      fixture.detectChanges();

      const req = httpMock.expectOne(`${environment.apiBaseUrl}/catalog/search?q=laptop`);
      expect(req.request.method).toBe('GET');
      expect(component.query).toBe('laptop');
      expect(component.loading).toBe(true);

      req.flush({ hits: [{ id: '1', name: 'Laptop' }], found: 1 });

      expect(component.products).toEqual([{ id: '1', name: 'Laptop' }]);
      expect(component.total).toBe(1);
      expect(component.loading).toBe(false);
    });

    it('should not search when query parameter is empty', () => {
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

      fixture.detectChanges();

      httpMock.expectNone(`${environment.apiBaseUrl}/catalog/search?q=`);
      expect(component.loading).toBe(false);
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

      const req = newHttpMock.expectOne(`${environment.apiBaseUrl}/catalog/search?q=laptop`);
      req.flush({ hits: [], found: 0 });
      expect(component.query).toBe('laptop');

      newHttpMock.verify();
    });

    it('should handle search errors gracefully', () => {
      fixture.detectChanges();

      const req = httpMock.expectOne(`${environment.apiBaseUrl}/catalog/search?q=laptop`);
      req.flush('Error', { status: 500, statusText: 'Server Error' });

      expect(component.products).toEqual([]);
      expect(component.total).toBe(0);
      expect(component.loading).toBe(false);
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

      httpMock.expectNone(`${environment.apiBaseUrl}/catalog/search`);
      expect(component.query).toBe('');
      expect(component.loading).toBe(false);
    });
  });
});
