import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CartHttpService } from './cart-http.service';
import { environment } from '../../../../environments/environment';
import { CartDto } from '../models/cart';

describe('CartHttpService', () => {
    let service: CartHttpService;
    let httpMock: HttpTestingController;
    const baseUrl = `${environment.apiBaseUrl}/Cart`;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [CartHttpService]
        });
        service = TestBed.inject(CartHttpService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get cart', () => {
        const mockCart: CartDto = {
            items: [],
            total: 0
        } as unknown as CartDto;

        service.getCart().subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(baseUrl);
        expect(req.request.method).toBe('GET');
        req.flush(mockCart);
    });

    it('should get cart when response has cart property', () => {
        const mockCart: CartDto = {
            items: [],
            total: 100
        } as unknown as CartDto;
        const response = { cart: mockCart };

        service.getCart().subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(baseUrl);
        req.flush(response);
    });

    it('should add to cart', () => {
        const productId = 'product123';
        const quantity = 2;
        const mockCart: CartDto = {
            items: [{ productId, quantity }],
            total: 200
        } as CartDto;

        service.addToCart(productId, quantity).subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(`${baseUrl}/items`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual({ productId, quantity });
        req.flush(mockCart);
    });

    it('should add to cart with default quantity of 1', () => {
        const productId = 'product123';
        const mockCart: CartDto = {
            items: [],
            total: 0
        } as unknown as CartDto;

        service.addToCart(productId).subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(`${baseUrl}/items`);
        expect(req.request.body).toEqual({ productId, quantity: 1 });
        req.flush(mockCart);
    });

    it('should add to cart when response has cart property', () => {
        const productId = 'product123';
        const mockCart: CartDto = { items: [], total: 0 } as unknown as CartDto;
        const response = { cart: mockCart };

        service.addToCart(productId).subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(`${baseUrl}/items`);
        req.flush(response);
    });

    it('should update quantity', () => {
        const productId = 'product123';
        const quantity = 5;
        const mockCart: CartDto = {
            items: [{ productId, quantity }],
            total: 500
        } as CartDto;

        service.updateQuantity(productId, quantity).subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(`${baseUrl}/items/${productId}`);
        expect(req.request.method).toBe('PUT');
        expect(req.request.body).toEqual({ productId, quantity });
        req.flush(mockCart);
    });

    it('should update quantity when response has cart property', () => {
        const productId = 'product123';
        const quantity = 3;
        const mockCart: CartDto = { items: [], total: 0 } as unknown as CartDto;
        const response = { cart: mockCart };

        service.updateQuantity(productId, quantity).subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(`${baseUrl}/items/${productId}`);
        req.flush(response);
    });

    it('should remove from cart', () => {
        const productId = 'product123';
        const mockCart: CartDto = {
            items: [],
            total: 0
        } as unknown as CartDto;

        service.removeFromCart(productId).subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(`${baseUrl}/items/${productId}`);
        expect(req.request.method).toBe('DELETE');
        req.flush(mockCart);
    });

    it('should remove from cart when response has cart property', () => {
        const productId = 'product123';
        const mockCart: CartDto = { items: [], total: 0 } as unknown as CartDto;
        const response = { cart: mockCart };

        service.removeFromCart(productId).subscribe(cart => {
            expect(cart).toEqual(mockCart);
        });

        const req = httpMock.expectOne(`${baseUrl}/items/${productId}`);
        req.flush(response);
    });
});
