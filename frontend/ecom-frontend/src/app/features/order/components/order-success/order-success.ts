import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-success',
  standalone: true,
  templateUrl: './order-success.html'
})
export class OrderSuccessComponent {
  private router = inject(Router);


  goToOrders() {
    this.router.navigate(['/orders']);
  }
}