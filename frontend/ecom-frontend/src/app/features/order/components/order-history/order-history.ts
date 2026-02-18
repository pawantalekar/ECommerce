import { Component, OnInit, inject } from '@angular/core';
import { Orderservice } from '../../services/orderservice';
import { Order } from '../../models/order';
import { Router, RouterModule } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterModule],
  templateUrl: './order-history.html',
  styleUrl: './order-history.css'
})
export class OrderHistoryComponent implements OnInit {
  private orderService = inject(Orderservice);
  private router = inject(Router);

  orders: Order[] = [];

  ngOnInit() {
    this.orderService.getMyOrders().subscribe(orders => this.orders = orders);
  }

  viewDetail(id: string) {
    this.router.navigate(['/orders', id]);
  }
}