import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import { Customer } from '../../models/customer';

@Component({
  selector: 'app-customer-table',
  standalone: true,
  templateUrl: './customer-table.html',
  styleUrl: './customer-table.scss'
})
export class CustomerTable {
  @Input() customers: Customer[] = [];
  @Input() sortBy = 'companyName';
  @Input() descending = false;
  @Input() deactivatingCustomerId: number | null = null;

  @Output() sortingChanged = new EventEmitter<string>();
  @Output() editRequested = new EventEmitter<number>();
  @Output() deactivateRequested =
    new EventEmitter<Customer>();

  changeSorting(column: string): void {
    this.sortingChanged.emit(column);
  }

  edit(id: number): void {
    this.editRequested.emit(id);
  }

  deactivate(customer: Customer): void {
    this.deactivateRequested.emit(customer);
  }
}
