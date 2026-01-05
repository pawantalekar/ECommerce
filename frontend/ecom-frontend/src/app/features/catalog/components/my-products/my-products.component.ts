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
import { DropdownModule } from 'primeng/dropdown';
import { ChipsModule } from 'primeng/chips';
import { FileUploadModule } from 'primeng/fileupload';

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
        DropdownModule,
        ChipsModule
    ],
    templateUrl: './my-products.html',
    styleUrls: ['./my-products.css']
})
export class MyProductsComponent implements OnInit {
[x: string]: any;
    @ViewChild('dt') table!: Table;

    products: ProductResult[] = [];
    loading = true;
    error: string | null = null;

    editDialogVisible = false;
    editingProduct: ProductForEdit | null = null;
    editLoading = false;

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
                    ...data,
                    shortDescription: data.shortDescription ?? '',
                    description: data.description ?? '',
                    tags: data.tags ?? [],
                    // Map images to imageUrls
                    imageUrls: Array.isArray(data.images)
                        ? data.images.map((img: any) => img.url)
                        : (data.imageUrls ?? []),
                    brandName: data.brandName ?? '',
                    brandId: data.brandId ?? '',
                    stockQuantity: data.stockQuantity ?? 0,
                    price: data.price ?? 0,
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
}