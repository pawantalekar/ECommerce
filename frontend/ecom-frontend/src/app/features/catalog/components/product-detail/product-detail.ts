import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Catalog } from '../../services/catalog';
import { Product } from '../../models/product';
import { AddToCartButtonComponent } from '../../../cart/components/add-to-cart-button/add-to-cart-button';
import { CartStore } from '../../../cart/signals/cart.store';
import { ReviewService } from '../../../reviews/services/review-service';


@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, AddToCartButtonComponent],
  templateUrl: './product-detail.html'
})
export class ProductDetail implements OnInit {
  product: Product | null = null;
  loading = true;
  error: string | null = null;
  primaryImage: string = 'https://via.placeholder.com/500';

  private route = inject(ActivatedRoute);
  private catalog = inject(Catalog);
  private cartStore = inject(CartStore);
  private router = inject(Router);
  private reviewService = inject(ReviewService);

  cartItems = this.cartStore.cart;
  canReview = false;
  reviews: any[] = [];
  averageRating = 0;

  ngOnInit(): void {
    const slug = this.route.snapshot.paramMap.get('slug');
    if (slug) this.load(slug);
    this.checkCanReview();
  }


  load(slug: string): void {
    this.loading = true;
    this.catalog.getBySlug(slug).subscribe({
      next: d => {
        this.product = d;
        this.loadReviews();
        this.primaryImage = d.imageUrls[0] || 'https://via.placeholder.com/500';
        this.loading = false;
        this.checkCanReview();
      },
      error: () => {
        this.error = 'Product not found.';
        this.loading = false;
      }
    });
  }
  private checkCanReview() {
    {
      if (!this.product) return;

      const productId = this.product.id;


      this.reviewService.getMyReview(productId).subscribe({
        next: (existing) => {
          if (existing) {
            this.canReview = true;
            return;
          }


          this.reviewService.canReview(productId).subscribe({
            next: (can) => this.canReview = can
          });
        },
        error: () => this.canReview = false
      });
    }
  }
  isInCart(): boolean {
    if (!this.product) return false;
    return this.cartItems().items.some(i => i.productId === this.product!.id);
  }

  goToCart() {
    this.router.navigate(['/cart']);
  }

  get galleryImages(): string[] {
    if (!this.product?.imageUrls) return [];
    return this.product.imageUrls.filter(url => url !== this.primaryImage);
  }


  setPrimaryImage(url: string): void {
    this.primaryImage = url;
  }
  directCheckout() {
    if (!this.product) return;
    this.router.navigate(['/checkout'], {
      state: {
        directBuy: true,
        productId: this.product.id,
        quantity: 1,
        productName: this.product.name,
        thumbnailUrl: this.product.imageUrls[0] || '',
        price: this.product.price
      }
    });
  }

  private loadReviews() {
    if (!this.product) return;
    this.reviewService.getReviews(this.product.id).subscribe({
      next: (data) => {
        this.reviews = data;
        if (data.length > 0) {
          this.averageRating = Math.round(
            data.reduce((sum: number, r: any) => sum + r.rating, 0) / data.length
          );
        }
      }
    });
  }
}