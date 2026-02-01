import { Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar implements OnInit {
  isLoggedIn = false;
  userRole = 'User';
  searchTerm = '';
  cartCount = 3;

  constructor(public auth: AuthService, private router: Router, private http: HttpClient) {

    this.auth.user$.subscribe(user => {
      this.isLoggedIn = !!user;
      if (user) {
        this.auth.loadUserRole();
      }
    });
    this.auth.role$.subscribe(role => {
      this.userRole = role;
    });
  }
  requestToBeSeller() {
    if (confirm('Do you want to become a seller?')) {
      this.http.post(`${environment.apiBaseUrl}/admin/seller-requests`, {}).subscribe({
        next: () => alert('Request sent! Waiting for admin approval.'),
        error: (err) => alert(err.error?.message || 'Could not send request.')
      });
    }
  }
  onSearch(): void {
    const term = this.searchTerm.trim();
    if (term) {
      this.router.navigate(['/search'], { queryParams: { q: term } });
      this.searchTerm = '';
    }
  }
  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      this.auth.loadUserRole();
      this.userRole = this.auth.getRole();
    }
  }
  logout(): void {
    this.auth.logout();
  }
}