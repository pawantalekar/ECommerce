import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { SsoCallback } from './features/auth/sso-callback/sso-callback';
import { Home } from './features/home/home/home';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'auth/login', component: Login },
    { path: 'auth/callback', component: SsoCallback }
];
