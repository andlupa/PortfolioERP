import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Category } from '../../../categories/models/category';

export interface ProductFilters {
  search: string;
  categoryId: number | null;
  status: 'active' | 'inactive' | 'all';
}

@Component({
  selector: 'app-product-filter',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './product-filter.html',
  styleUrl: './product-filter.scss'
})
export class ProductFilter {
  
  @Input() categories: Category[] = [];

  @Output() filtersApplied = new EventEmitter<ProductFilters>();
  @Output() filtersReset = new EventEmitter<void>();
  @Output() searchChanged = new EventEmitter<string>();

  search = '';
  categoryId: number | null = null;
  status: 'active' | 'inactive' | 'all' = 'active';

  onSearchChanged(value: string): void {
    this.searchChanged.emit(value);
  }

  apply(): void {
    this.filtersApplied.emit({
      search: this.search,
      categoryId: this.categoryId,
      status: this.status
    });
  }

  reset(): void {
    this.search = '';
    this.categoryId = null;
    this.status = 'active';

    this.searchChanged.emit('');
    this.filtersReset.emit();
  }

}
