import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';

// entities
import { Category } from '../../../categories/models/category';
import { Product } from '../../models/product';

// services
import { CategoryService } from '../../../categories/services/category.service';
import { ProductService } from '../../services/product.service';

// children components
import { ProductFilter, ProductFilters } from '../../components/product-filter/product-filter';
import { ProductPagination } from '../../components/product-pagination/product-pagination';
import { ProductTable } from '../../components/product-table/product-table';
import { ProductToolbar } from '../../components/product-toolbar/product-toolbar';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { AppToast, ToastType } from '../../../../shared/components/app-toast/app-toast';

import {
  EMPTY,
  Subject,
  catchError,
  debounceTime,
  distinctUntilChanged,
  finalize,
  switchMap
} from 'rxjs';

import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

// questa classe gestisce il componente HTML app-product-list-page
@Component({
  selector: 'app-product-list-page',
  standalone: true,
  imports: [
    ProductToolbar,
    ProductFilter,
    ProductTable,
    ProductPagination,
    ConfirmDialog,
    AppToast
  ],
  templateUrl: './product-list-page.html',
  styleUrl: './product-list-page.scss'
})
export class ProductListPage implements OnInit {
  // oggetti che verranno creati da Angular
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  
  // Signals, rendono l'interfaccia reattiva
  readonly products = signal<Product[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  // Signals per disattivazione prodotto
  readonly productToDeactivate = signal<Product | null>(null);
  readonly deactivatingProductId = signal<number | null>(null);
  readonly deactivationMessage = computed(() => {
    const product = this.productToDeactivate();

    if (!product) {
      return '';
    }

    return `Vuoi disattivare il prodotto "${product.name}" (${product.code})?`;
  });
  // Signals per lo stato del toast
  readonly toastVisible = signal(false);
  readonly toastMessage = signal('');
  readonly toastType = signal<ToastType>('success');

  private toastTimer: ReturnType<typeof setTimeout> | null = null;
  private readonly search$ = new Subject<string>();
  private readonly DestroyRef = inject(DestroyRef);
  

  // search parameters variables
  search = '';
  categoryId: number | null = null;
  status: 'active' | 'inactive' | 'all' = 'active';
  sortBy = 'name';
  descending = false;

  // pagination 
  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;

  // LIFECICLE - caricamento iniziale dei dati
  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();

    this.search$
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),

        switchMap(search => {
          this.search = search;
          this.pageNumber = 1;

          this.loading.set(true);
          this.error.set(null);

          let isActive: boolean | undefined;

          if (this.status === 'active') {
            isActive = true;
          } else if (this.status === 'inactive') {
            isActive = false;
          }

          return this.productService.getProducts({
            search: this.search,
            categoryId: this.categoryId ?? undefined,
            isActive,
            sortBy: this.sortBy,
            descending: this.descending,
            pageNumber: this.pageNumber,
            pageSize: this.pageSize
          }).pipe(
            catchError(error => {
              console.error(
                'Errore ricerca prodotti',
                error
              );

              this.error.set(
                'Impossibile caricare i prodotti.'
              );

              return EMPTY;
            }),

            finalize(() => {
              this.loading.set(false);
            })
          );
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: response => {
          this.products.set(response.items);
          this.totalItems = response.totalItems;
          this.totalPages = response.totalPages;
        }
      });
  }

  loadCategories(): void {
    this.categoryService.getCategories(false).subscribe({
      next: categories => this.categories.set(categories),
      error: error => {
        console.error('Errore caricamento categorie', error);
      }
    });
  }

  loadProducts(): void {
    this.loading.set(true);
    this.error.set(null);

    let isActive: boolean | undefined;

    if (this.status === 'active') {
      isActive = true;
    } else if (this.status === 'inactive') {
      isActive = false;
    }

    this.productService.getProducts({
      search: this.search,
      categoryId: this.categoryId ?? undefined,
      isActive,
      sortBy: this.sortBy,
      descending: this.descending,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    }).subscribe({
      next: response => {
        this.products.set(response.items);
        this.totalItems = response.totalItems;
        this.totalPages = response.totalPages;
        this.loading.set(false);
      },
      error: error => {
        console.error('Errore caricamento prodotti', error);
        this.error.set('Impossibile caricare i prodotti.');
        this.loading.set(false);
      }
    });
  }

  applyFilters(filters: ProductFilters): void {
    this.search = filters.search;
    this.categoryId = filters.categoryId;
    this.status = filters.status;
    this.pageNumber = 1;

    this.loadProducts();
  }

  resetFilters(): void {
    this.search = '';
    this.categoryId = null;
    this.status = 'active';
    this.pageNumber = 1;

    this.loadProducts();
  }

  changeSorting(column: string): void {
    if (this.sortBy === column) {
      this.descending = !this.descending;
    } else {
      this.sortBy = column;
      this.descending = false;
    }

    this.pageNumber = 1;
    this.loadProducts();
  }

  changePage(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.loadProducts();
  }

  createProduct(): void {
    void this.router.navigate(['/products/new']);
  }

  editProduct(productId: number): void {
    void this.router.navigate(['/products', productId, 'edit']);
  }

  deactivateProduct(product: Product): void {
    this.productToDeactivate.set(product);
  }

  cancelDeactivation(): void {
    if (this.deactivatingProductId() !== null) {
      return;
    }

    this.productToDeactivate.set(null);
  }

  confirmDeactivation(): void {
    const product = this.productToDeactivate();

    if (!product) {
      return;
    }

    this.deactivatingProductId.set(product.id);
    this.error.set(null);

    this.productService.deactivateProduct(product.id).subscribe({
      next: () => {
        this.deactivatingProductId.set(null);
        this.productToDeactivate.set(null);

        this.showToast(
          `Il prodotto "${product.name}" è stato disattivato.`,
          'success'
        );

        this.loadProducts();
      },
      error: error => {
        console.error('Errore disattivazione prodotto', error);

        this.deactivatingProductId.set(null);
        this.productToDeactivate.set(null);

        const message =
          error.status === 404
            ? 'Il prodotto non è stato trovato.'
            : 'Impossibile disattivare il prodotto.';

        this.showToast(message, 'danger');
      }
    });
  }

  closeToast(): void {
    this.toastVisible.set(false);

    if(this.toastTimer) {
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

    console.log("Toast:", message);

    this.toastMessage.set(message);
    this.toastType.set(type);
    this.toastVisible.set(true);

    this.toastTimer = setTimeout(() => {
      this.toastVisible.set(false);
      this.toastTimer = null;
    }, 4000);
  }

  onSearchChanged(search: string): void {
    this.search$.next(search);
  }
}

