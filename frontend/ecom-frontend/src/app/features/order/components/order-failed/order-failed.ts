import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-failed',
  standalone: true,
  templateUrl: './order-failed.html'
})
export class OrderFailedComponent {
  private router = inject(Router);


  goToOrders() {
    this.router.navigate(['/orders']);
  }
}