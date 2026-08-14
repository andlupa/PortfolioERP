import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { Router } from '@angular/router';

import {
  AppToast,
  ToastType
} from '../../../../shared/components/app-toast/app-toast';
import {
  ConfirmDialog
} from '../../../../shared/components/confirm-dialog/confirm-dialog';
import {
  CustomerFilter,
  CustomerFilters
} from '../../components/customer-filter/customer-filter';
import {
  CustomerPagination
} from '../../components/customer-pagination/customer-pagination';
import {
  CustomerTable
} from '../../components/customer-table/customer-table';
import {
  CustomerToolbar
} from '../../components/customer-toolbar/customer-toolbar';
import { Customer } from '../../models/customer';
import { CustomerService } from '../../services/customer.service';

@Component({
  selector: 'app-customer-list-page',
  standalone: true,
  imports: [
    CustomerToolbar,
    CustomerFilter,
    CustomerTable,
    CustomerPagination,
    ConfirmDialog,
    AppToast
  ],
  templateUrl: './customer-list-page.html',
  styleUrl: './customer-list-page.scss'
})
export class CustomerListPage implements OnInit {
  private readonly customerService = inject(CustomerService);
  private readonly router = inject(Router);

  readonly customers = signal<Customer[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly customerToDeactivate =
    signal<Customer | null>(null);

  readonly deactivatingCustomerId =
    signal<number | null>(null);

  readonly toastVisible = signal(false);
  readonly toastMessage = signal('');
  readonly toastType = signal<ToastType>('success');

  search = '';
  city = '';
  country = '';
  status: 'active' | 'inactive' | 'all' = 'active';

  sortBy = 'companyName';
  descending = false;

  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;

  private toastTimer:
    ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.loadCustomers();
  }

  get deactivationMessage(): string {
    const customer = this.customerToDeactivate();

    return customer
      ? `Vuoi disattivare il cliente "${customer.companyName}"?`
      : '';
  }

  loadCustomers(): void {
    this.loading.set(true);
    this.error.set(null);

    let isActive: boolean | undefined;

    if (this.status === 'active') {
      isActive = true;
    } else if (this.status === 'inactive') {
      isActive = false;
    }

    this.customerService.getCustomers({
      search: this.search,
      city: this.city,
      country: this.country,
      isActive,
      sortBy: this.sortBy,
      descending: this.descending,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    }).subscribe({
      next: response => {
        this.customers.set(response.items);
        this.totalItems = response.totalItems;
        this.totalPages = response.totalPages;
        this.loading.set(false);
      },
      error: error => {
        console.error('Errore caricamento clienti', error);
        this.error.set('Impossibile caricare i clienti.');
        this.loading.set(false);
      }
    });
  }

  applyFilters(filters: CustomerFilters): void {
    this.search = filters.search;
    this.city = filters.city;
    this.country = filters.country;
    this.status = filters.status;
    this.pageNumber = 1;

    this.loadCustomers();
  }

  resetFilters(): void {
    this.search = '';
    this.city = '';
    this.country = '';
    this.status = 'active';
    this.pageNumber = 1;

    this.loadCustomers();
  }

  changeSorting(column: string): void {
    if (this.sortBy === column) {
      this.descending = !this.descending;
    } else {
      this.sortBy = column;
      this.descending = false;
    }

    this.pageNumber = 1;
    this.loadCustomers();
  }

  changePage(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.loadCustomers();
  }

  createCustomer(): void {
    void this.router.navigate(['/customers/new']);
  }

  editCustomer(customerId: number): void {
    void this.router.navigate([
      '/customers',
      customerId,
      'edit'
    ]);
  }

  requestDeactivation(customer: Customer): void {
    this.customerToDeactivate.set(customer);
  }

  cancelDeactivation(): void {
    if (this.deactivatingCustomerId() === null) {
      this.customerToDeactivate.set(null);
    }
  }

  confirmDeactivation(): void {
    const customer = this.customerToDeactivate();

    if (!customer) {
      return;
    }

    this.deactivatingCustomerId.set(customer.id);
    this.error.set(null);

    this.customerService
      .deactivateCustomer(customer.id)
      .subscribe({
        next: () => {
          this.deactivatingCustomerId.set(null);
          this.customerToDeactivate.set(null);

          this.showToast(
            `Il cliente "${customer.companyName}" è stato disattivato.`,
            'success'
          );

          this.loadCustomers();
        },
        error: error => {
          console.error(
            'Errore disattivazione cliente',
            error
          );

          this.deactivatingCustomerId.set(null);
          this.customerToDeactivate.set(null);

          this.showToast(
            error.status === 404
              ? 'Cliente non trovato.'
              : 'Impossibile disattivare il cliente.',
            'danger'
          );
        }
      });
  }

  closeToast(): void {
    this.toastVisible.set(false);

    if (this.toastTimer) {
      clearTimeout(this.toastTimer);
      this.toastTimer = null;
    }
  }

  private showToast(
    message: string,
    type: ToastType
  ): void {
    if (this.toastTimer) {
      clearTimeout(this.toastTimer);
    }

    this.toastMessage.set(message);
    this.toastType.set(type);
    this.toastVisible.set(true);

    this.toastTimer = setTimeout(() => {
      this.toastVisible.set(false);
      this.toastTimer = null;
    }, 4000);
  }
}
