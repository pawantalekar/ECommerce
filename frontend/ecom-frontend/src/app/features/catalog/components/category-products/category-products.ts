import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Catalog } from '../../services/catalog';
import { Product } from '../../models/product';
import { ProductCard } from "../product-card/product-card";

@Component({
  selector: 'app-category-products',
  standalone: true,
  imports: [CommonModule, ProductCard],
  templateUrl: './category-products.html',
  styleUrls: ['./category-products.css'],
})
export class CategoryProducts implements OnInit {
  products: Product[] = [];
  loading = true;
  error: string | null = null;
  categoryId: string | null = null;

  private route = inject(ActivatedRoute);
  private catalog = inject(Catalog);

  ngOnInit(): void {
    console.log('Category products:', this.products);
    this.categoryId = this.route.snapshot.paramMap.get('categoryId');
    if (this.categoryId) {
      this.fetchProductsByCategory(this.categoryId);
    } else {
      this.error = 'No category specified.';
      this.loading = false;
    }
  }

  fetchProductsByCategory(categoryId: string) {
    this.loading = true;
    this.catalog.getProductsByCategory(categoryId).subscribe({
      next: (products) => {
        // Ensure imageUrls is always a non-null array
        console.log('Fetched category products:', products);
        this.products = products.map(p => ({
          ...p,
          imageUrls: Array.isArray(p.imageUrls) ? p.imageUrls : []
        }));
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load products.';
        this.loading = false;
      }
    });
  }
}
