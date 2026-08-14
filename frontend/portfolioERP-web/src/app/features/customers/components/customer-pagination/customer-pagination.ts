import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'app-customer-pagination',
  standalone: true,
  templateUrl: './customer-pagination.html',
  styleUrl: './customer-pagination.scss'
})
export class CustomerPagination {
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
