import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './navbar.html'
})
export class Navbar {
  isLoggedIn = false;

  constructor(public auth: AuthService, private router: Router) {
    this.auth.user$.subscribe(user => this.isLoggedIn = !!user);
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/home']);
  }
}
