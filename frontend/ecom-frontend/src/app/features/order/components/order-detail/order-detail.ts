import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';  
import { CommonModule } from '@angular/common';
import { Orderservice } from '../../services/orderservice';
import { Order } from '../../models/order';
import { CartHttpService } from '../../../cart/services/cart-http.service';
import { ReviewService } from '../../../reviews/services/review-service';   

@Component({
  selector: 'app-order-detail',
  standalone: true,
  templateUrl: './order-detail.html',
  imports: [CommonModule, RouterModule]
})
export class OrderDetailComponent implements OnInit {
  order: Order | null = null;
  canReviewMap = new Map<string, boolean>();

  constructor(
    private route: ActivatedRoute,
    private orderService: Orderservice,
    private cartService: CartHttpService,
    private router: Router,
    private reviewService: ReviewService  
  ) { }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.orderService.getOrderById(id).subscribe(order => {
        this.order = order;

        if (this.order?.paymentStatus === 'Paid') {
          this.checkReviewStatusForItems(this.order.items.map(item => item.productId));
        }
      });
    }
  }

  goToReview(productId: string, productName: string) {
   
    const slug = productName
      .toLowerCase()
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/(^-|-$)/g, '');

    this.router.navigate(['/catalog/products', slug, 'review'], {
      state: { productId: productId }
    });
  }
  private checkReviewStatusForItems(productIds: string[]) {
    productIds.forEach(pid => {
      this.reviewService.getMyReview(pid).subscribe({
        next: (review) => this.canReviewMap.set(pid, true),
        error: () => {
          this.reviewService.canReview(pid).subscribe(can => {
            this.canReviewMap.set(pid, can);
          });
        }
      });
    });
  }

  canUserReview(productId: string): boolean {
    return this.canReviewMap.get(productId) === true;
  }

  buyAgain() {
    if (!this.order) return;

    const orderItems = this.order.items.map(item => ({
      productId: item.productId,
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