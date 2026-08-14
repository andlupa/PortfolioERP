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
  CategoryTable
} from '../../components/category-table/category-table';
import {
  CategoryToolbar
} from '../../components/category-toolbar/category-toolbar';
import { Category } from '../../models/category';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-category-list-page',
  standalone: true,
  imports: [
    CategoryToolbar,
    CategoryTable,
    ConfirmDialog,
    AppToast
  ],
  templateUrl: './category-list-page.html',
  styleUrl: './category-list-page.scss'
})
export class CategoryListPage implements OnInit {
  private readonly categoryService =
    inject(CategoryService);

  private readonly router = inject(Router);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly includeInactive = signal(false);

  readonly categoryToDeactivate =
    signal<Category | null>(null);

  readonly deactivatingCategoryId =
    signal<number | null>(null);

  readonly toastVisible = signal(false);
  readonly toastMessage = signal('');
  readonly toastType = signal<ToastType>('success');

  private toastTimer:
    ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.loadCategories();
  }

  get deactivationMessage(): string {
    const category = this.categoryToDeactivate();

    return category
      ? `Vuoi disattivare la categoria "${category.name}"?`
      : '';
  }

  loadCategories(): void {
    this.loading.set(true);
    this.error.set(null);

    this.categoryService
      .getCategories(this.includeInactive())
      .subscribe({
        next: categories => {
          this.categories.set(categories);
          this.loading.set(false);
        },
        error: error => {
          console.error(
            'Errore caricamento categorie',
            error
          );

          this.error.set(
            'Impossibile caricare le categorie.'
          );

          this.loading.set(false);
        }
      });
  }

  toggleInactive(): void {
    this.includeInactive.update(value => !value);
    this.loadCategories();
  }

  createCategory(): void {
    void this.router.navigate(['/categories/new']);
  }

  editCategory(id: number): void {
    void this.router.navigate([
      '/categories',
      id,
      'edit'
    ]);
  }

  requestDeactivation(category: Category): void {
    this.categoryToDeactivate.set(category);
  }

  cancelDeactivation(): void {
    if (this.deactivatingCategoryId() === null) {
      this.categoryToDeactivate.set(null);
    }
  }

  confirmDeactivation(): void {
    const category = this.categoryToDeactivate();

    if (!category) {
      return;
    }

    this.deactivatingCategoryId.set(category.id);

    this.categoryService
      .deactivateCategory(category.id)
      .subscribe({
        next: () => {
          this.deactivatingCategoryId.set(null);
          this.categoryToDeactivate.set(null);

          this.showToast(
            `La categoria "${category.name}" è stata disattivata.`,
            'success'
          );

          this.loadCategories();
        },
        error: error => {
          console.error(
            'Errore disattivazione categoria',
            error
          );

          this.deactivatingCategoryId.set(null);
          this.categoryToDeactivate.set(null);

          this.showToast(
            error.status === 404
              ? 'Categoria non trovata.'
              : 'Impossibile disattivare la categoria.',
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
