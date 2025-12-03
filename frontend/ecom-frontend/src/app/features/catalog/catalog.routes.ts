
import { Routes } from '@angular/router';

export const CATALOG_ROUTES: Routes = [
    {
        path: 'products',
        loadComponent: () => import('./components/product-list/product-list')
            .then(m => m.ProductList)
    },
    {
        path: 'products/:slug',
        loadComponent: () => import('./components/product-detail/product-detail')
            .then(m => m.ProductDetail),
    },  
    {
        path: 'products/:slug/review',           
        loadComponent: () => import('../reviews/components/write-review-page/write-review-page')
            .then(c => c.WriteReviewPageComponent)
    },    
  
    {
        path: 'add',
        loadComponent: () => import('./components/add-product/add-product')
            .then(m => m.AddProduct)
    },
   
];