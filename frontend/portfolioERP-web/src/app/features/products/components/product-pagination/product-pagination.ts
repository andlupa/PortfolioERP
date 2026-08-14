import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-product-pagination',
  standalone: true,
  templateUrl: './product-pagination.html',
  styleUrl: './product-pagination.scss'
})
export class ProductPagination {
  @Input() pageNumber = 1;
  @Input() totalPages = 0;
  @Input() totalItems = 0;

  @Output() pageChanged = new EventEmitter<number>();

  previous(): void {
    if (this.pageNumber > 1) {
      this.pageChanged.emit(this.pageNumber - 1);
    }
  }

  next(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageChanged.emit(this.pageNumber + 1);
    }
  }
}
