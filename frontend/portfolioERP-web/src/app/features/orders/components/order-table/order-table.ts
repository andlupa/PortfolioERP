import { CommonModule } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import { OrderListItem } from '../../models/order-list-item';
import {
  getOrderStatusLabel,
  OrderStatus
} from '../../models/order-status';

@Component({
  selector: 'app-order-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-table.html',
  styleUrl: './order-table.scss'
})
export class OrderTable {
  @Input() orders: OrderListItem[] = [];
  @Input() sortBy = 'orderDate';
  @Input() descending = true;

  @Output() sortingChanged = new EventEmitter<string>();
  @Output() detailRequested = new EventEmitter<number>();
  @Output() confirmRequested =
    new EventEmitter<OrderListItem>();
  @Output() cancelRequested =
    new EventEmitter<OrderListItem>();

  readonly OrderStatus = OrderStatus;

  changeSorting(column: string): void {
    this.sortingChanged.emit(column);
  }

  getStatusLabel(status: OrderStatus): string {
    return getOrderStatusLabel(status);
  }

  statusClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Draft:
        return 'text-bg-secondary';

      case OrderStatus.Confirmed:
        return 'text-bg-primary';

      case OrderStatus.Processing:
        return 'text-bg-warning';

      case OrderStatus.Shipped:
        return 'text-bg-info';

      case OrderStatus.Completed:
        return 'text-bg-success';

      case OrderStatus.Cancelled:
        return 'text-bg-danger';

      default:
        return 'text-bg-secondary';
    }
  }
}
