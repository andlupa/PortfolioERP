import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { Product } from '../../models/product';

@Component({
  selector: 'app-product-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-table.html',
  styleUrl: './product-table.scss'
})
export class ProductTable {
  @Input() products: Product[] = [];
  @Input() sortBy = 'name';
  @Input() descending = false;
  @Input() deactivatingProductId: number | null = null;

  @Output() sortingChanged = new EventEmitter<string>();
  @Output() editRequested = new EventEmitter<number>();
  @Output() deactivateRequested = new EventEmitter<Product>();

  changeSorting(column: string): void {
    this.sortingChanged.emit(column);
  }

  edit(productId: number): void {
    this.editRequested.emit(productId);
  }

  deactivate(product: Product): void {
    this.deactivateRequested.emit(product);
  }
}
