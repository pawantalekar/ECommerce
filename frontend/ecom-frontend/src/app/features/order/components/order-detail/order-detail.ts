import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Orderservice } from '../../services/orderservice';
import { Order } from '../../models/order';
import { CartHttpService } from '../../../cart/services/cart-http.service';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  templateUrl: './order-detail.html',
  imports: [CommonModule]
})
export class OrderDetailComponent implements OnInit {
  order: Order | null = null;

  constructor(private route: ActivatedRoute, private orderService: Orderservice, private cartService: CartHttpService, private router: Router) { }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.orderService.getOrderById(id).subscribe(order => this.order = order);
  }

  buyAgain() {
    if (!this.order) return;

    const orderItems = this.order.items.map(item => ({
      productId: item.productId || item.productId,
      quantity: item.quantity,
      productName: item.productName,
      thumbnailUrl: item.thumbnailUrl,
      unitPrice: item.unitPrice
    }));

    this.router.navigate(['/checkout'], {
      state: {
        directBuyFromOrder: true,
        items: orderItems
      }
    });
  }
}