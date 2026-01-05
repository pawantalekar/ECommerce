import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BreadcrumbService {
    private category = new BehaviorSubject<{ id: string, name: string } | null>(null);
    category$ = this.category.asObservable();

    setCategory(category: { id: string, name: string } | null) {
        this.category.next(category);
    }
}