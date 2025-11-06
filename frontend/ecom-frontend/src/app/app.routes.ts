import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { SsoCallback } from './features/auth/sso-callback/sso-callback';

export const routes: Routes = [
    { path: 'auth/login', component: Login },
    { path: 'auth/callback', component: SsoCallback },
    { path: '**', redirectTo: 'auth/login' }
];
