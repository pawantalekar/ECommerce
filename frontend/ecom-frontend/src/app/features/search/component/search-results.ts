import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductSearchService } from '../services/product-search.service';
import { AsyncPipe, CommonModule } from '@angular/common';
import { ProductCard } from '../../catalog/components/product-card/product-card';
import { PaginatorModule } from 'primeng/paginator';

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [AsyncPipe, ProductCard, PaginatorModule],
  templateUrl: './search-results.html',
  styleUrls: ['./search-results.css']
})
export class SearchResults implements OnInit {
  query = '';
  result$;   

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private searchService: ProductSearchService   
  ) {
    this.result$ = this.searchService.result$;
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.query = params['q']?.trim() || '';
      const page = Number(params['page']) || 1;
      this.searchService.search(this.query, page);
    });
  }

  onPageChange(event: any): void {
    const page = event.page + 1;
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: page > 1 ? page : null },
      queryParamsHandling: 'merge'
    });
  }
}