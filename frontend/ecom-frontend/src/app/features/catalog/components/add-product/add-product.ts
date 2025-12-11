
import { Component, inject } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Catalog, AddProductPayload } from '../../services/catalog';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './add-product.html',
  styleUrls: ['./add-product.css']
})
export class AddProduct {
  private catalog = inject(Catalog);
  private router = inject(Router);

  model: AddProductPayload = {
    name: '',
    shortDescription: '',
    description: '',
    price: 0,
    sku: '',
    stockQuantity: 0,
    imageUrls: [],
    tags: [],
    categoryName: '',
    categorySlug: '',
    brandName: '',
    brandLogoUrl: ''
  };

  tagInput = '';
  selectedFiles: File[] = [];
  uploading = false;
  error = '';

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files);
    }
  }

  private updateTags() {
    this.model.tags = this.tagInput
      .split(',')
      .map(t => t.trim())
      .filter(t => t.length > 0);
  }

  async onSubmit() {
    this.updateTags();


    if (!this.model.name || !this.model.sku || this.model.price <= 0 || this.model.stockQuantity <= 0) {
      this.error = 'Please fill all required fields.';
      return;
    }
    if (!this.model.categoryName || !this.model.brandName) {
      this.error = 'Category and Brand are required.';
      return;
    }

    this.error = '';
    this.uploading = true;

    try {

      const urls: string[] = [];
      for (const file of this.selectedFiles) {
        const resp = await this.catalog.uploadImage(file).toPromise();
        if (resp?.url) {
          urls.push(resp.url);
        }
      }
      this.model.imageUrls = urls;


      await this.catalog.addProduct(this.model).toPromise();


      this.router.navigate(['/catalog/products']);
    } catch (err: any) {
      this.error = err?.error?.error || 'Failed to add product';
    } finally {
      this.uploading = false;
    }
  }
}