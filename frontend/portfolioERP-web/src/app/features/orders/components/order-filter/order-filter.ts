import {
  Component,
  EventEmitter,
  Output
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { OrderStatus } from '../../models/order-status';

export interface OrderFilters {
  search: string;
  status: OrderStatus | null;
  dateFrom: string;
  dateTo: string;
}

@Component({
  selector: 'app-order-filter',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './order-filter.html',
  styleUrl: './order-filter.scss'
})
export class OrderFilter {
  @Output() filtersApplied =
    new EventEmitter<OrderFilters>();

  @Output() filtersReset = new EventEmitter<void>();

  readonly OrderStatus = OrderStatus;

  search = '';
  status: OrderStatus | null = null;
  dateFrom = '';
  dateTo = '';

  apply(): void {
    this.filtersApplied.emit({
      search: this.search,
      status: this.status,
      dateFrom: this.dateFrom,
      dateTo: this.dateTo
    });
  }

  reset(): void {
    this.search = '';
    this.status = null;
    this.dateFrom = '';
    this.dateTo = '';

    this.filtersReset.emit();
  }
}
