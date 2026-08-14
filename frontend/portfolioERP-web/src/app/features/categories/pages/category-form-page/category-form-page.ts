import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { CreateCategoryRequest } from '../../models/create-category-request';
import { UpdateCategoryRequest } from '../../models/update-category-request';
import { CategoryService } from '../../services/category.service';
import { FormValidationService } from '../../../../shared/services/form-validation.service';
import { ValidationProblemDetails } from '../../../../shared/models/validation-problem-details';

@Component({
  selector: 'app-category-form-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './category-form-page.html',
  styleUrl: './category-form-page.scss'
})
export class CategoryFormPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly formValidationService = inject(FormValidationService);

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  categoryId: number | null = null;

  readonly form = this.formBuilder.nonNullable.group({
    name: [
      '',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],
    description: [
      '',
      Validators.maxLength(500)
    ],
    isActive: [true]
  });

  get isEditMode(): boolean {
    return this.categoryId !== null;
  }

  get pageTitle(): string {
    return this.isEditMode
      ? 'Modifica categoria'
      : 'Nuova categoria';
  }

  ngOnInit(): void {
    this.formValidationService
      .clearBackendErrorsOnValueChanges(this.form);

    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (Number.isInteger(id) && id > 0) {
      this.categoryId = id;
      this.loadCategory(id);
    }
  }

  loadCategory(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.categoryService.getCategoryById(id).subscribe({
      next: category => {
        this.form.patchValue({
          name: category.name,
          description: category.description ?? '',
          isActive: category.isActive
        });

        this.loading.set(false);
      },
      error: error => {
        console.error('Errore caricamento categoria', error);

        this.error.set(
          error.status === 404
            ? 'Categoria non trovata.'
            : 'Impossibile caricare la categoria.'
        );

        this.loading.set(false);
      }
    });
  }

  save(): void {
    this.error.set(null);
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    this.saving.set(true);

    const value = this.form.getRawValue();

    if (this.categoryId === null) {
      const request: CreateCategoryRequest = {
        name: value.name.trim(),
        description: this.normalizeOptional(value.description)
      };

      this.categoryService.createCategory(request).subscribe({
        next: () => {
          void this.router.navigate(['/categories']);
        },
        error: error => this.handleSaveError(error)
      });

      return;
    }

    const request: UpdateCategoryRequest = {
      name: value.name.trim(),
      description: this.normalizeOptional(value.description),
      isActive: value.isActive
    };

    this.categoryService
      .updateCategory(this.categoryId, request)
      .subscribe({
        next: () => {
          void this.router.navigate(['/categories']);
        },
        error: error => this.handleSaveError(error)
      });
  }

  private handleSaveError(error: HttpErrorResponse): void {
    console.error('Errore salvataggio categoria', error);

    if (error.status === 400) {
      this.formValidationService.applyBackendErrors(
        this.form,
        error.error as ValidationProblemDetails
      );
      this.error.set('Controlla i dati inseriti.');
    } else if (error.status === 409) {
      this.error.set(
        error.error?.detail ??
        'Esiste già una categoria con lo stesso nome.'
      );
    } else {
      this.error.set('Impossibile salvare la categoria.');
    }

    this.saving.set(false);
  }

  private normalizeOptional(value: string): string | null {
    const normalized = value.trim();
    return normalized.length > 0 ? normalized : null;
  }
}
