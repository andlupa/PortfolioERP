import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Category } from '../../../categories/models/category';
import { CategoryService } from '../../../categories/services/category.service';

import { finalize } from 'rxjs';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductList implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);

  readonly categories = signal<Category[]>([]);

  selectedCategoryId: number | null = null;
  selectedStatus = 'active';

  sortBy = 'name';
  descending = false;
  readonly products = signal<Product[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  search = '';
  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.categoryService
      .getCategories(false)
      .subscribe({
        next: categories => {
          this.categories.set(categories);
        },
        error: error => {
          console.error('Unable to load categories', error);
        }
      });
  }

  loadProducts(): void {
    this.loading.set(true);
    this.error.set(null);

    let isActive: boolean | undefined;

    if (this.selectedStatus === 'active') {
      isActive = true;
    } else if (this.selectedStatus === 'inactive') {
      isActive = false;
    }
    
    this.productService.getProducts({
      search: this.search,
      categoryId: this.selectedCategoryId ?? undefined,
      isActive,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      descending: this.descending
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
      next: response => {
        this.products.set(response.items);
        this.totalItems = response.totalItems;
        this.totalPages = response.totalPages;
       {
}      },
      error: error => {
        console.error('Errore caricamento prodotti:', error);
        this.error.set(
          `Impossibile caricare i prodotti. HTTP ${error.status}`
        );
      }
    });
  }

  searchProducts(): void {
    this.pageNumber = 1;
    this.loadProducts();
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.loadProducts();
    }
  }

  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.loadProducts();
    }
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadProducts();
  }

  resetFilters(): void {
    this.search = '';
    this.selectedCategoryId = null;
    this.selectedStatus = 'active';
    this.sortBy = 'name';
    this.descending = false;
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
}
