
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductList } from '../../catalog/components/product-list/product-list';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ProductList],
  templateUrl: './home.html'
})
export class Home { }