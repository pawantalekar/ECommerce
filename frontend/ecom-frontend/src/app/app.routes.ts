import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { SsoCallback } from './features/auth/sso-callback/sso-callback';
import { Home } from './features/home/home/home';
import { ApprovalsComponent } from './features/admin/approvals/approvals';
import { AdminGuard } from './core/guards/admin-guard';
import { AuthGuard } from './core/guards/auth-guard';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home, data: { breadcrumb: 'Home' } },
    { path: 'auth/login', component: Login, data: { breadcrumb: 'Login' } },
    { path: 'auth/callback', component: SsoCallback, data: { breadcrumb: 'SSO Callback' } },

    {
        path: 'catalog',
        loadChildren: () => import('./features/catalog/catalog.routes').then(m => m.CATALOG_ROUTES),
       data: { breadcrumb: 'Catalog' }
    },
    {
        path: 'cart',
        loadChildren: () => import('./features/cart/cart.routes').then(m => m.CART_ROUTES),
        data: { breadcrumb: 'Cart' },
        canActivate: [AuthGuard]
    },
    {
        path: '',
        loadChildren: () => import('./features/order/order.routes').then(m => m.ORDER_ROUTES),
        data: { breadcrumb: 'Orders' }
    },
    {
        path: 'search',
        loadComponent: () => import('./features/search/component/search-results').then(m => m.SearchResults),
        data: { breadcrumb: 'Search Results' }
    },
    {
        path: 'admin/approvals',
        component: ApprovalsComponent,
        canActivate: [AdminGuard],
        data: { breadcrumb: 'Approvals' }
    }
];