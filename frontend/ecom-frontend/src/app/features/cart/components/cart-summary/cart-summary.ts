import { Component, inject } from '@angular/core';
import { CartStore } from '../../signals/cart.store';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-cart-summary',
  standalone: true,
  imports: [DecimalPipe, RouterModule],
  templateUrl: './cart-summary.html',
  styleUrls: ['./cart-summary.css']
})
export class CartSummaryComponent {
  store = inject(CartStore);
}