// src/app/features/catalog/components/product-detail/product-detail.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Catalog } from '../../services/catalog';
import { Product } from '../../models/product';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail.html'
})
export class ProductDetail implements OnInit {
  product: Product | null = null;
  loading = true;
  error: string | null = null;

  
  primaryImage: string = 'https://via.placeholder.com/500';

  constructor(private route: ActivatedRoute, private catalog: Catalog) { }

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

  
  get galleryImages(): string[] {
    if (!this.product?.imageUrls) return [];
    return this.product.imageUrls.filter(url => url !== this.primaryImage);
  }

  
  setPrimaryImage(url: string): void {
    this.primaryImage = url;
  }
}