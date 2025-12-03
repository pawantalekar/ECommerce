import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth-guard';

export const ORDER_ROUTES: Routes = [
    {
        path: 'checkout',
        loadComponent: () => import('./components/checkout/checkout').then(c => c.CheckoutComponent),
        canActivate: [AuthGuard]
    },
    {
        path: 'order/success',
        loadComponent: () => import('./components/order-success/order-success').then(c => c.OrderSuccessComponent),
        canActivate: [AuthGuard]
    },
    {
        path: 'order/failed',
        loadComponent: () => import('./components/order-failed/order-failed').then(c => c.OrderFailedComponent)
    },
    {
        path: 'orders',
        loadComponent: () => import('./components/order-history/order-history').then(c => c.OrderHistoryComponent),
        canActivate: [AuthGuard]
    },
    {
        path: 'orders/:id',
        loadComponent: () => import('./components/order-detail/order-detail').then(c => c.OrderDetailComponent),
        canActivate: [AuthGuard]
    }
];