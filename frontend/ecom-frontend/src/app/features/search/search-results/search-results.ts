// src/app/features/search/search-results/search-results.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './search-results.html',
  styleUrls: ['./search-results.css']
})
export class SearchResults implements OnInit {
  query = '';
  products: any[] = [];
  total = 0;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.query = params['q']?.trim() || '';
      if (this.query) this.search();
      else this.loading = false;
    });
  }

  private search(): void {
    this.loading = true;
    const url = `${environment.apiBaseUrl}/catalog/search?q=${encodeURIComponent(this.query)}`;
    this.http.get<any>(url).subscribe({
      next: (res) => {
        this.products = res.hits || [];
        this.total = res.found || 0;
        this.loading = false;
      },
      error: () => {
        this.products = [];
        this.total = 0;
        this.loading = false;
      }
    });
  }
}