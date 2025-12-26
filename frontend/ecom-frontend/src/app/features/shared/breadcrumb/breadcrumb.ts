
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { filter, Subscription } from 'rxjs';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { CommonModule } from '@angular/common';
import { BreadcrumbService } from './breadcrumb.service';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.html',
  styleUrls: ['./breadcrumb.css'],
  imports: [BreadcrumbModule, CommonModule]
})
export class BreadcrumbComponent implements OnInit, OnDestroy {
  items: MenuItem[] = [];
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/home' };
  private category: { id: string, name: string } | null = null;
  private categorySub: Subscription | undefined;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private breadcrumbService: BreadcrumbService
  ) { }

  ngOnInit() {
    this.categorySub = this.breadcrumbService.category$.subscribe(category => {
      this.category = category;
      this.items = this.createBreadcrumbs(this.activatedRoute.root);
    });

    this.router.events.pipe(
      filter((event: any) => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.items = this.createBreadcrumbs(this.activatedRoute.root);
    });
    // Initial load
    this.items = this.createBreadcrumbs(this.activatedRoute.root);
  }

  ngOnDestroy() {
    this.categorySub?.unsubscribe();
  }

  private createBreadcrumbs(route: ActivatedRoute, url = '', breadcrumbs: MenuItem[] = []): MenuItem[] {
    const children: ActivatedRoute[] = route.children;
    if (children.length === 0) {
      // Check for custom parent in route data
      const parent = route.snapshot.data['parent'];
      if (parent) {
        breadcrumbs.unshift(parent);
      }
      // Insert category before Product Details if present
      if (this.category && breadcrumbs.length > 0 && breadcrumbs[breadcrumbs.length - 1].label === 'Product Details') {
        breadcrumbs.splice(breadcrumbs.length - 1, 0, {
          label: this.category.name,
          routerLink: `/catalog/category/${this.category.id}`
        });
      }
      // Filter out intermediate 'Orders' breadcrumb if present
      return breadcrumbs.filter(b => b.label !== 'Orders');
    }
    for (const child of children) {
      const routeURL: string = (child.snapshot.url ?? []).map(segment => segment.path).join('/');
      if (routeURL !== '') {
        url += `/${routeURL}`;
      }
      const label = child.snapshot.data['breadcrumb'];
      if (label) {
        breadcrumbs.push({ label, routerLink: url });
      }
      return this.createBreadcrumbs(child, url, breadcrumbs);
    }
    return breadcrumbs;
  }
}


