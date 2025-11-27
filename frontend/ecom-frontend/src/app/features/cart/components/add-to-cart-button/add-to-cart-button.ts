import { Component, Input, inject } from '@angular/core';
import { CartStore } from '../../signals/cart.store';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-to-cart-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button class="btn btn-primary btn-lg me-2 position-relative"
            (click)="addToCart()"
            [disabled]="loading()">
      <span class="spinner-border spinner-border-sm me-2" *ngIf="loading()"></span>
      <i class="bi bi-cart-plus"></i>
      <span>Add to Cart</span>
    </button>
  `,
  styles: [
    `button:hover { transform: translateY(-1px); }`
  ]
})
export class AddToCartButtonComponent {
  @Input({ required: true }) productId!: string | { toString(): string };
  @Input() quantity = 1;

  private cartStore = inject(CartStore);
  private router = inject(Router);
  private auth = inject(AuthService);

  loading = this.cartStore.loading;

  addToCart() {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/auth/login'], {
        queryParams: { returnUrl: this.router.url }
      });
      return;
    }

    const id = typeof this.productId === 'string' ? this.productId : this.productId.toString();
    this.cartStore.addToCart(id, this.quantity);
  }
}