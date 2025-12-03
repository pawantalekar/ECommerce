import { Component, OnInit } from '@angular/core';
import { Orderservice } from '../../services/orderservice';
import { Order } from '../../models/order';
import { Router } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './order-history.html'
})
export class OrderHistoryComponent implements OnInit {
  orders: Order[] = [];

  constructor(private orderService: Orderservice, private router: Router) { }

  ngOnInit() {
    this.orderService.getMyOrders().subscribe(orders => this.orders = orders);
  }

  viewDetail(id: string) {
    this.router.navigate(['/orders', id]);
  }
}