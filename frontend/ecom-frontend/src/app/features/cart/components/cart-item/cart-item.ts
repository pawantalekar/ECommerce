import { Component, Input, inject } from '@angular/core';
import { CartItemDto } from '../../models/cart';
import { CartStore } from '../../signals/cart.store';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterModule } from '@angular/router';

@Component({
  selector: 'app-cart-item',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, RouterModule ],
  templateUrl: './cart-item.html',
  styleUrls: ['./cart-item.css']
})
export class CartItemComponent {
  @Input({ required: true }) item!: CartItemDto;

  private cartStore = inject(CartStore);
  loading = this.cartStore.loading;

  updateQuantity(qty: number) {
    if (qty < 1) return;
    this.cartStore.updateQuantity(this.item.productId, qty);
  }

  remove() {
    this.cartStore.removeFromCart(this.item.productId);
  }
}