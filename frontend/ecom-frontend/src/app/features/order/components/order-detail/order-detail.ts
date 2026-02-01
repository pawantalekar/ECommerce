import { Component, OnInit, inject } from '@angular/core';
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
  styleUrls: ['./order-detail.css'],
  imports: [CommonModule, RouterModule]
})
export class OrderDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private orderService = inject(Orderservice);
  private cartService = inject(CartHttpService);
  private router = inject(Router);
  private reviewService = inject(ReviewService);

  order: Order | null = null;
  canReviewMap = new Map<string, boolean>();
  showFullTimeline = false;

  statusConfig: Record<string, { icon: string; color: string; label: string }> = {
    'Pending': { icon: 'bi-clock-history', color: '#9CA3AF', label: 'Order Placed' },
    'Confirmed': { icon: 'bi-check-circle', color: '#3B82F6', label: 'Confirmed' },
    'Processing': { icon: 'bi-box-seam', color: '#F59E0B', label: 'Processing' },
    'Shipped': { icon: 'bi-truck', color: '#06B6D4', label: 'Shipped' },
    'OutForDelivery': { icon: 'bi-geo-alt', color: '#8B5CF6', label: 'Out for Delivery' },
    'Delivered': { icon: 'bi-check-circle-fill', color: '#10B981', label: 'Delivered' },
    'Cancelled': { icon: 'bi-x-circle', color: '#EF4444', label: 'Cancelled' },
    'Failed': { icon: 'bi-exclamation-triangle', color: '#EF4444', label: 'Failed' }
  };

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
        next: () => this.canReviewMap.set(pid, true),
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

  getStatusConfig(status: string) {
    return this.statusConfig[status] || { icon: 'bi-circle', color: '#6c757d', label: status };
  }

  isStatusCompleted(status: string): boolean {
    if (!this.order?.statusHistory) return false;
    return this.order.statusHistory.some(h => h.status === status);
  }

  getStatusDate(status: string): Date | null {
    if (!this.order?.statusHistory) return null;
    const history = this.order.statusHistory.find(h => h.status === status);
    return history ? new Date(history.changedAt) : null;
  }

  getStatusNotes(status: string): string | null {
    if (!this.order?.statusHistory) return null;
    const history = this.order.statusHistory.find(h => h.status === status);
    return history?.notes || null;
  }

  toggleFullTimeline() {
    this.showFullTimeline = !this.showFullTimeline;
  }

  getCompactTimeline() {
    if (!this.order?.statusHistory || this.order.statusHistory.length === 0) return [];
    if (this.order.statusHistory.length === 1) return this.order.statusHistory;
    return [
      this.order.statusHistory[0],
      this.order.statusHistory[this.order.statusHistory.length - 1]
    ];
  }
}
