import { AuthService } from '../../../core/services/auth-service';
import { environment } from '../../../../environments/environment';
import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: true,
  template: `<button (click)="loginWithGoogle()">Sign in with Google</button>`
})
export class Login {
  constructor(private auth: AuthService) { }

  loginWithGoogle() {
    window.location.href = `${environment.apiBaseUrl}/Auth/google-login`;
  }
}
