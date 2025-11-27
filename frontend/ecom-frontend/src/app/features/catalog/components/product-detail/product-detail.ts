import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Catalog } from '../../services/catalog';
import { Product } from '../../models/product';
import { AddToCartButtonComponent } from '../../../cart/components/add-to-cart-button/add-to-cart-button';
import { CartStore } from '../../../cart/signals/cart.store';


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

  cartItems = this.cartStore.cart;

  ngOnInit(): void {
    const slug = this.route.snapshot.paramMap.get('slug');
    if (slug) this.load(slug);
  }

  load(slug: string): void {
    this.loading = true;
    this.catalog.getBySlug(slug).subscribe({
      next: d => {
        this.product = d;
        this.primaryImage = d.imageUrls[0] || 'https://via.placeholder.com/500';
        this.loading = false;
      },
      error: () => {
        this.error = 'Product not found.';
        this.loading = false;
      }
    });
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
}