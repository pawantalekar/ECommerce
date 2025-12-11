// src/app/features/catalog/components/product-list/product-list.ts
import { Component, OnInit, inject } from '@angular/core';

import { ProductCard } from '../product-card/product-card';
import { Catalog } from '../../services/catalog';
import { Product } from '../../models/product';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth-service'; // Import AuthService

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [ProductCard, RouterModule],
  templateUrl: './product-list.html'
})
export class ProductList implements OnInit {
  private catalog = inject(Catalog);
  private auth = inject(AuthService);

  products: Product[] = [];
  loading = true;
  error: string | null = null;

  
  isLoggedIn = false;

  constructor() {
    
    this.auth.user$.subscribe(user => this.isLoggedIn = !!user);
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;
    this.catalog.getAll().subscribe({
      next: d => { this.products = d; this.loading = false; },
      error: () => { this.error = 'Failed to load products.'; this.loading = false; }
    });
  }
}