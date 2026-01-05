import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { Navbar } from './features/shared/navbar/navbar';


import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import { BreadcrumbComponent } from "./features/shared/breadcrumb/breadcrumb";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterModule, Navbar, BreadcrumbComponent],
  templateUrl: './app.html',
})
export class App { }
