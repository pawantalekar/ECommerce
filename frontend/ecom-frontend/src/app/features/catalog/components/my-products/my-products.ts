import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Table, TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { BadgeModule } from 'primeng/badge';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { Catalog, ProductResult } from '../../services/catalog';
import { FormsModule } from '@angular/forms';
import { ProductForEdit } from '../../models/product';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { FileUploadModule } from 'primeng/fileupload';
import { ChipsModule } from 'primeng/chips';


@Component({
  selector: 'app-my-products',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    ToggleButtonModule,
    BadgeModule,
    InputTextModule,
    FormsModule,
    DialogModule,
    InputTextareaModule,
    InputNumberModule,
    ProgressSpinnerModule,
    FileUploadModule,
    ChipsModule
  ],
  templateUrl: './my-products.html',
  styleUrls: ['./my-products.css']
})
export class MyProductsComponent implements OnInit {
  @ViewChild('dt') table!: Table;

  products: ProductResult[] = [];
  loading = true;
  error: string | null = null;

  editDialogVisible = false;
  editingProduct: ProductForEdit | null = null;
  editLoading = false;
  imageUploading = false;

  constructor(private catalogService: Catalog) { }

  ngOnInit(): void {
    this.loadMyProducts();
  }

  loadMyProducts(): void {
    this.loading = true;
    this.error = null;

    this.catalogService.getMyProducts().subscribe({
      next: (data: ProductResult[]) => {
        this.products = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load your products. Please try again later.';
        this.loading = false;
      }
    });
  }

  onToggleChange(product: ProductResult): void {
    this.catalogService.toggleProductActive(product.id).subscribe({
      next: (response: { isActive: boolean }) => {
        product.isActive = response.isActive;
      },
      error: () => {
        product.isActive = !product.isActive;
        alert('Failed to update status');
      }
    });
  }

  onGlobalFilter(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.table.filterGlobal(value, 'contains');
  }

  openEditModal(product: ProductResult): void {
    this.editLoading = true;
    this.editDialogVisible = true;

    this.catalogService.GetProductForEdit(product.id).subscribe({
      next: (data: any) => {
        this.editingProduct = {
          id: data.id,
          name: data.name,
          shortDescription: data.shortDescription || '',
          description: data.description || '',
          price: data.price,
          sku: data.sku,
          stockQuantity: data.stockQuantity,
          categoryId: data.categoryId,
          categoryName: data.categoryName,
          brandId: data.brandId || null,
          brandName: data.brandName || '',
          brandLogoUrl: data.brandLogoUrl || '',
          isActive: data.isActive,
          isFeatured: data.isFeatured,
          tags: Array.isArray(data.tags) ? data.tags.map((t: any) => typeof t === 'string' ? t : t.name) : [],
          // Critical fix: map images array to imageUrls array of strings
          imageUrls: data.images ? data.images.map((img: any) => img.url) : []
        };
        this.editLoading = false;
      },
      error: () => {
        alert('Failed to load product for editing');
        this.editDialogVisible = false;
      }
    });
  }

  closeEditModal(): void {
    this.editDialogVisible = false;
    this.editingProduct = null;
  }

  saveEdit(): void {
    if (!this.editingProduct) return;

    this.catalogService.updateProduct(this.editingProduct.id, this.editingProduct).subscribe({
      next: () => {
        alert('Product updated successfully');
        this.closeEditModal();
        this.loadMyProducts();
      },
      error: () => {
        alert('Failed to update product');
      }
    });
  }

  onImageUpload(event: any): void {
    const files: FileList = event.target.files;
    if (files.length === 0) return;

    this.imageUploading = true;

    Array.from(files).forEach(file => {
      this.catalogService.uploadImage(file).subscribe({
        next: (response: { url: string }) => {
          this.editingProduct!.imageUrls.push(response.url);
        },
        error: () => {
          alert(`Failed to upload ${file.name}`);
        },
        complete: () => {
          this.imageUploading = false;
        }
      });
    });

    event.target.value = '';
  }

  removeImage(index: number): void {
    this.editingProduct!.imageUrls.splice(index, 1);
  }
}