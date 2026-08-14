import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {  HttpErrorResponse } from '@angular/common/http';

import { Category } from '../../../categories/models/category';
import { CategoryService } from '../../../categories/services/category.service';
import { CreateProductRequest } from '../../models/create-product-request';
import { UpdateProductRequest } from '../../models/update-product-request';
import { ProductService } from '../../services/product.service';
import { FormValidationService } from '../../../../shared/services/form-validation.service';
import { ValidationProblemDetails } from '../../../../shared/models/validation-problem-details';

import {
  debounceTime,
  distinctUntilChanged
} from 'rxjs';

@Component({
  selector: 'app-product-form-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './product-form-page.html',
  styleUrl: './product-form-page.scss'
})
export class ProductFormPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly formValidationService = inject(FormValidationService);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  productId: number | null = null;

  readonly form = this.formBuilder.nonNullable.group({
    code: [
      '',
      [
        Validators.required,
        Validators.maxLength(50)
      ]
    ],
    name: [
      '',
      [
        Validators.required,
        Validators.maxLength(150)
      ]
    ],
    description: [
      '',
      Validators.maxLength(1000)
    ],
    price: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],
    stockQuantity: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],
    categoryId: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],
    vatPercentage: [
      22,
      [
        Validators.required,
        Validators.min(0),
        Validators.max(100)
      ]
    ],
    isActive: [true]
  });

  get isEditMode(): boolean {
    return this.productId !== null;
  }

  get pageTitle(): string {
    return this.isEditMode
      ? 'Modifica prodotto'
      : 'Nuovo prodotto';
  }

  ngOnInit(): void {
    this.formValidationService.clearBackendErrorsOnValueChanges(this.form);

    this.form.controls.code.valueChanges
      .pipe(
        //debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(code => {

        const normalizedCode =
          code.toUpperCase();

        if (code !== normalizedCode) {

          this.form.controls.code.setValue(
            normalizedCode,
            {
              emitEvent: false
            }
          );

        }

      });

    this.loadCategories();

    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (Number.isInteger(id) && id > 0) {
      this.productId = id;
      this.loadProduct(id);
    }
  }

  loadCategories(): void {
    this.categoryService.getCategories(false).subscribe({
      next: categories => {
        this.categories.set(categories);
      },
      error: error => {
        console.error('Errore caricamento categorie', error);
        this.error.set('Impossibile caricare le categorie.');
      }
    });
  }

  loadProduct(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.productService.getProductById(id).subscribe({
      next: product => {
        this.form.patchValue({
          code: product.code,
          name: product.name,
          description: product.description ?? '',
          price: product.price,
          stockQuantity: product.stockQuantity,
          categoryId: product.categoryId,
          vatPercentage: product.vatPercentage,
          isActive: product.isActive
        });

        this.loading.set(false);
      },
      error: error => {
        console.error('Errore caricamento prodotto', error);

        if (error.status === 404) {
          this.error.set('Prodotto non trovato.');
        } else {
          this.error.set('Impossibile caricare il prodotto.');
        }

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

    if (this.productId === null) {
      const request: CreateProductRequest = {
        code: value.code.trim(),
        name: value.name.trim(),
        description: this.normalizeOptional(value.description),
        price: value.price,
        stockQuantity: value.stockQuantity,
        categoryId: value.categoryId,
        vatPercentage: value.vatPercentage
      };

      this.productService.createProduct(request).subscribe({
        next: () => {
          void this.router.navigate(['/products']);
        },
        error: error => {
          this.handleSaveError(error);
        }
      });

      return;
    }

    const request: UpdateProductRequest = {
      code: value.code.trim(),
      name: value.name.trim(),
      description: this.normalizeOptional(value.description),
      price: value.price,
      stockQuantity: value.stockQuantity,
      categoryId: value.categoryId,
      vatPercentage: value.vatPercentage,
      isActive: value.isActive
    };

    this.productService
      .updateProduct(this.productId, request)
      .subscribe({
        next: () => {
          void this.router.navigate(['/products']);
        },
        error: error => {
          this.handleSaveError(error);
        }
      });
  }

  private handleSaveError(error: HttpErrorResponse): void {
    console.error('Errore salvataggio prodotto', error);

    if (error.status === 400) {
      this.formValidationService.applyBackendErrors(
        this.form,
        error.error as ValidationProblemDetails
      );
      this.error.set('Controlla i dati inseriti.');
    } else if (error.status === 409) {
      this.error.set(
        error.error?.detail ??
        'Esiste già un prodotto con lo stesso codice.'
      );
    } else {
      this.error.set('Impossibile salvare il prodotto.');
    }

    this.saving.set(false);
  }

  private normalizeOptional(value: string): string | null {
    const normalized = value.trim();
    return normalized.length > 0 ? normalized : null;
  }
}
