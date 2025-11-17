import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Login } from './login/login';
import { SsoCallback } from './sso-callback/sso-callback';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [],
  imports: [CommonModule, RouterModule, Login, SsoCallback],
  exports: [Login, SsoCallback]
})
export class AuthModule { }
