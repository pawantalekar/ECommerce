import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-success',
  standalone: true,
  templateUrl: './order-success.html'
})
export class OrderSuccessComponent {
  constructor(private router: Router) { }

  goToOrders() {
    this.router.navigate(['/orders']);
  }
}