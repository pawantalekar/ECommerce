
import { Component, OnInit, inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProductList } from '../../catalog/components/product-list/product-list';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
import { Catalog } from '../../catalog/services/catalog';
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { ToastModule } from 'primeng/toast';
import { CartStore } from '../../cart/signals/cart.store';
import { AuthService } from '../../../core/services/auth-service';

interface FeaturedProduct {
  id: string;
  name: string;
  slug: string;
  price: number;
  shortDescription: string | null;
  categoryName: string;
  brandName: string;
  imageUrls: string[];
}
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ProductList, CommonModule, ProductList, CarouselModule, ButtonModule, RouterModule, ProgressSpinnerModule, ToastModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css'],
  providers: [MessageService]
})
export class Home implements OnInit {
  featuredProducts: FeaturedProduct[] = [];
  loadingFeatured = true;
  responsiveOptions = [
    {
      breakpoint: '1199px',
      numVisible: 3,
      numScroll: 1
    },
    {
      breakpoint: '991px',
      numVisible: 2,
      numScroll: 1
    },
    {
      breakpoint: '767px',
      numVisible: 1,
      numScroll: 1
    }
  ];

  private cartStore = inject(CartStore);
  private messageService = inject(MessageService);
  private authService = inject(AuthService);

  constructor(private catalogService: Catalog, private router: Router) { }

  ngOnInit(): void {
    this.loadFeaturedProducts();
  }

  loadFeaturedProducts(): void {
    this.catalogService.GetFeaturedProducts().subscribe({
      next: (products) => {
        this.featuredProducts = products.map(p => ({
          id: p.id,
          name: p.name,
          slug: p.slug,
          price: p.price,
          categoryName: p.categoryName,
          brandName: p.brandName,
          shortDescription: p.shortDescription,
          imageUrls: p.imageUrls
        }));
        this.loadingFeatured = false;
      },
      error: () => {
        this.loadingFeatured = false;
        // Optional: show error message
      }
    });
  }

  goToProductDetail(slug: string) {
    this.router.navigate(['/catalog/products', slug]);
  }

  addToCart(product: FeaturedProduct, event: Event) {
    event.stopPropagation();
    if (!this.authService.isLoggedIn()) {
      this.showError('Login first to add products to cart.');
      return;
    }
    const cartItems = this.cartStore.cart().items;
    const alreadyInCart = cartItems.some(item => item.productId === product.id);
    if (alreadyInCart) {
      this.showError('Product already in the cart.');
      return;
    }
    this.cartStore.addToCart(product.id, 1);
    this.showSuccess(product.name);
  }

  showError(msg: string) {
    this.messageService.add({ severity: 'warn', summary: 'Warning', detail: msg });
  }

  showSuccess(productName: string) {
    this.messageService.add({ severity: 'success', summary: 'Added to Cart', detail: `${productName} added to cart!` });
  }
}