import { Component, inject, OnInit } from '@angular/core';
import { CartStore } from '../../signals/cart.store';
import { CartItemComponent } from '../cart-item/cart-item';
import { CartSummaryComponent } from '../cart-summary/cart-summary';


@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CartItemComponent, CartSummaryComponent],
  templateUrl: './cart-page.html',
  styleUrls: ['./cart-page.css']
})
export class CartPageComponent implements OnInit {
  store = inject(CartStore);

  ngOnInit() {
    this.store.loadCart();
  }
}