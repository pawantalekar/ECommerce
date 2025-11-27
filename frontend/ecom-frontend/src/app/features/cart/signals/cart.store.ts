import { Injectable, signal, computed, inject } from '@angular/core';
import { CartHttpService } from '../services/cart-http.service';
import { CartDto } from '../models/cart';

@Injectable({
    providedIn: 'root'
})
export class CartStore {
    private state = signal<CartDto>({
        id: '',
        itemsCount: 0,
        total: 0,
        items: []
    });

    private _loading = signal(false);
    private http = inject(CartHttpService);

    readonly cart = this.state.asReadonly();
    readonly loading = this._loading.asReadonly();
    readonly itemCount = computed(() => this.state().itemsCount);
    readonly total = computed(() => this.state().total);

    constructor() {
        this.loadCart();
    }

    loadCart() {
        this._loading.set(true);
        this.http.getCart().subscribe({
            next: (cart) => {
                this.state.set(cart);
                this._loading.set(false);
            },
            error: () => this._loading.set(false)
        });
    }

    addToCart(productId: string, quantity = 1) {
        this.http.addToCart(productId, quantity).subscribe({
            next: (cart) => this.state.set(cart)
        });
    }

    updateQuantity(productId: string, quantity: number) {
        if (quantity < 1) return;

        const current = this.state();
        const item = current.items.find(i => i.productId === productId);
        if (!item) return;

        const qtyDiff = quantity - item.quantity;

        this.state.update(s => ({
            ...s,
            items: s.items.map(i =>
                i.productId === productId
                    ? { ...i, quantity }
                    : i
            ),
            total: s.total + qtyDiff * item.price,
            itemsCount: s.itemsCount + qtyDiff
        }));

        this._loading.set(true);
        this.http.updateQuantity(productId, quantity).subscribe({
            next: () => this._loading.set(false),
            error: () => {
                this._loading.set(false);
                this.loadCart();
            }
        });
    }

    removeFromCart(productId: string) {
        const current = this.state();
        const item = current.items.find(i => i.productId === productId);
        if (!item) return;

        this.state.update(s => ({
            ...s,
            items: s.items.filter(i => i.productId !== productId),
            total: s.total - item.price * item.quantity,
            itemsCount: s.itemsCount - item.quantity
        }));

        this._loading.set(true);
        this.http.removeFromCart(productId).subscribe({
            next: () => this._loading.set(false),
            error: () => {
                this._loading.set(false);
                this.loadCart();
            }
        });
    }

    clearCart() {
        this.state.set({ id: '', itemsCount: 0, total: 0, items: [] });
    }
}