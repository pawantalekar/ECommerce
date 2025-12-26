import { Routes } from '@angular/router';
import { CartPageComponent } from './components/cart-page/cart-page';

export const CART_ROUTES: Routes = [
    {
        path: '',
        component: CartPageComponent,
        data: { breadcrumb: 'Shopping Cart' }
    }
];