
import { Routes } from '@angular/router';

export const CATALOG_ROUTES: Routes = [
    {
        path: '',
        redirectTo: 'products',
        pathMatch: 'full'
    },
    {
        path: 'products',
        loadComponent: () => import('./components/product-list/product-list')
            .then(m => m.ProductList),
             data: { breadcrumb: 'Products' }
    },
    {
        path: 'products/:slug',
        loadComponent: () => import('./components/product-detail/product-detail')
            .then(m => m.ProductDetail),
        data: { breadcrumb: 'Product Details' }
    },  
    {
        path: 'products/:slug/review',           
        loadComponent: () => import('../reviews/components/write-review-page/write-review-page')
            .then(c => c.WriteReviewPageComponent),
        data: { breadcrumb: 'Write Review' }
    },
  
    {
        path: 'add',
        loadComponent: () => import('./components/add-product/add-product')
            .then(m => m.AddProduct),
        data: { breadcrumb: 'Add Product' }
    },
    {
        path: 'my-products',
        loadComponent: () => import('./components/my-products/my-products')
            .then(m => m.MyProductsComponent),
        data: { breadcrumb: 'My Products' }
    },
    {
        path: 'category/:categoryId',
        loadComponent: () => import('./components/category-products/category-products')
            .then(m => m.CategoryProducts),
        data: { breadcrumb: 'Category' }
    }
    
];