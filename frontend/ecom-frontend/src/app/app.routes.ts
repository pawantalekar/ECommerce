import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { SsoCallback } from './features/auth/sso-callback/sso-callback';
import { Home } from './features/home/home/home';
import { ApprovalsComponent } from './features/admin/approvals/approvals';
import { AdminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'auth/login', component: Login },
    { path: 'auth/callback', component: SsoCallback },

    {
        path: 'catalog',
        loadChildren: () => import('./features/catalog/catalog.routes').then(m => m.CATALOG_ROUTES)
    },
    {
        path: 'cart',
        loadChildren: () => import('./features/cart/cart.routes').then(m => m.CART_ROUTES)
    },
    {
        path: '',
        loadChildren: () => import('./features/order/order.routes').then(m => m.ORDER_ROUTES)
    },
    {
        path: 'search',
        loadComponent: () => import('./features/search/search-results/search-results')
            .then(m => m.SearchResults)
    },
    {
        path: 'admin/approvals',
        component: ApprovalsComponent,
        canActivate: [AdminGuard] 
    }
];