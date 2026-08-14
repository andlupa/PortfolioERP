import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { Supplier } from '../../models/supplier';
import { SupplierService } from '../../services/supplier.service';

@Component({
  selector: 'app-supplier-list-page',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink
  ],
  templateUrl: './supplier-list-page.html',
  styleUrl: './supplier-list-page.scss'
})
export class SupplierListPage implements OnInit {
  private readonly supplierService =
    inject(SupplierService);

  readonly suppliers = signal<Supplier[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly totalItems = signal(0);
  readonly totalPages = signal(0);

  search = '';
  city = '';
  country = '';
  status = '';

  pageNumber = 1;
  pageSize = 10;

  sortBy = 'companyName';
  descending = false;

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers(): void {
    this.loading.set(true);
    this.error.set(null);

    let isActive: boolean | undefined;

    if (this.status === 'active') {
      isActive = true;
    } else if (this.status === 'inactive') {
      isActive = false;
    }

    this.supplierService
      .getSuppliers({
        search: this.search,
        city: this.city,
        country: this.country,
        isActive,
        sortBy: this.sortBy,
        descending: this.descending,
        pageNumber: this.pageNumber,
        pageSize: this.pageSize
      })
      .subscribe({
        next: response => {
          this.suppliers.set(response.items);
          this.totalItems.set(response.totalItems);
          this.totalPages.set(response.totalPages);
          this.loading.set(false);
        },

        error: error => {
          console.error(
            'Errore caricamento fornitori',
            error
          );

          this.error.set(
            'Impossibile caricare i fornitori.'
          );

          this.loading.set(false);
        }
      });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadSuppliers();
  }

  resetFilters(): void {
    this.search = '';
    this.city = '';
    this.country = '';
    this.status = '';
    this.pageNumber = 1;

    this.loadSuppliers();
  }

  changeSort(column: string): void {
    if (this.sortBy === column) {
      this.descending = !this.descending;
    } else {
      this.sortBy = column;
      this.descending = false;
    }

    this.pageNumber = 1;
    this.loadSuppliers();
  }

  previousPage(): void {
    if (this.pageNumber <= 1) {
      return;
    }

    this.pageNumber--;
    this.loadSuppliers();
  }

  nextPage(): void {
    if (this.pageNumber >= this.totalPages()) {
      return;
    }

    this.pageNumber++;
    this.loadSuppliers();
  }

  deactivate(supplier: Supplier): void {
    if (!supplier.isActive) {
      return;
    }

    const confirmed = window.confirm(
      `Vuoi disattivare il fornitore "${supplier.companyName}"?`
    );

    if (!confirmed) {
      return;
    }

    this.supplierService
      .deactivate(supplier.id)
      .subscribe({
        next: () => {
          this.loadSuppliers();
        },

        error: error => {
          console.error(
            'Errore disattivazione fornitore',
            error
          );

          this.error.set(
            'Impossibile disattivare il fornitore.'
          );
        }
      });
  }
}
