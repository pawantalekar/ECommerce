// src/app/features/catalog/components/product-list/product-list.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductCard } from '../product-card/product-card';
import { Catalog } from '../../services/catalog';
import { Product } from '../../models/product';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth-service'; // Import AuthService

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, ProductCard, RouterModule],
  templateUrl: './product-list.html'
})
export class ProductList implements OnInit {
  products: Product[] = [];
  loading = true;
  error: string | null = null;

  
  isLoggedIn = false;

  constructor(
    private catalog: Catalog,
    private auth: AuthService 
  ) {
    
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