import {
  Component,
  EventEmitter,
  Output
} from '@angular/core';
import { FormsModule } from '@angular/forms';

export interface CustomerFilters {
  search: string;
  city: string;
  country: string;
  status: 'active' | 'inactive' | 'all';
}

@Component({
  selector: 'app-customer-filter',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './customer-filter.html',
  styleUrl: './customer-filter.scss'
})
export class CustomerFilter {
  @Output() filtersApplied =
    new EventEmitter<CustomerFilters>();

  @Output() filtersReset = new EventEmitter<void>();

  search = '';
  city = '';
  country = '';
  status: 'active' | 'inactive' | 'all' = 'active';

  apply(): void {
    this.filtersApplied.emit({
      search: this.search,
      city: this.city,
      country: this.country,
      status: this.status
    });
  }

  reset(): void {
    this.search = '';
    this.city = '';
    this.country = '';
    this.status = 'active';

    this.filtersReset.emit();
  }
}
