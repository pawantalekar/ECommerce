import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './navbar.html'
})
export class Navbar {
  isLoggedIn = false;
  searchTerm = '';
  cartCount = 3; // Replace with real cart service later

  constructor(public auth: AuthService, private router: Router) {
    this.auth.user$.subscribe(user => this.isLoggedIn = !!user);
  }

  onSearch(): void {
    const term = this.searchTerm.trim();
    if (term) {
      this.router.navigate(['/search'], { queryParams: { q: term } });
      this.searchTerm = '';
    }
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/home']);
  }
}
