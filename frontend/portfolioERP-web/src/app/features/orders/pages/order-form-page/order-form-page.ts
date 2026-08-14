import { CommonModule } from '@angular/common';
import {
  Component,
  DestroyRef,
  inject,
  OnInit,
  signal
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { Customer } from '../../../customers/models/customer';
import { CustomerService } from '../../../customers/services/customer.service';
import { Product } from '../../../products/models/product';
import { ProductService } from '../../../products/services/product.service';
import { FormValidationService } from '../../../../shared/services/form-validation.service';
import { ValidationProblemDetails } from '../../../../shared/models/validation-problem-details';
import { CreateOrderRequest } from '../../models/create-order-request';
import { OrderService } from '../../services/order.service';
import { HttpErrorResponse } from '@angular/common/http';
import {   debounceTime, distinctUntilChanged, filter, map, switchMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { OrderCalculationResponse } from '../../models/order-calculation-response';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-order-form-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './order-form-page.html',
  styleUrl: './order-form-page.scss'
})
export class OrderFormPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly customerService = inject(CustomerService);
  private readonly productService = inject(ProductService);
  private readonly orderService = inject(OrderService);
  private readonly formValidationService =
    inject(FormValidationService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly customers = signal<Customer[]>([]);
  readonly products = signal<Product[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly calculation = signal<OrderCalculationResponse | null>(null);
  readonly calculating = signal(false);
  readonly calculationError = signal<string | null>(null);

  readonly form = this.formBuilder.nonNullable.group({
    customerId: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],
    notes: [
      '',
      Validators.maxLength(1000)
    ],
    lines: this.formBuilder.array<FormGroup>([])
  });
  get lines(): FormArray<FormGroup> {
    return this.form.controls.lines;
  }

  ngOnInit(): void {
    this.formValidationService
      .clearBackendErrorsOnValueChanges(this.form);

    this.loadLookups();
    this.addLine();
    this.setupAutomaticCalculation();
  }

  loadLookups(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      customers: this.customerService.getActiveCustomers(),
      products: this.productService.getActiveProducts()
    }).subscribe({
      next: response => {
        this.customers.set(response.customers.items);
        this.products.set(response.products.items);
        this.loading.set(false);
      },
      error: error => {
        console.error('Errore caricamento dati ordine', error);
        this.error.set(
          'Impossibile caricare clienti e prodotti.'
        );
        this.loading.set(false);
      }
    });
  }

  addLine(): void {
    const line = this.formBuilder.nonNullable.group({
      productId: [
        0,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],
      quantity: [
        1,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],
      discountPercentage: [
        0,
        [
          Validators.required,
          Validators.min(0),
          Validators.max(100)
        ]
      ]
    });

    this.lines.push(line);
  }

  removeLine(index: number): void {
    if (this.lines.length <= 1) {
      return;
    }

    this.lines.removeAt(index);
  }

  getProduct(productId: number): Product | undefined {
    return this.products().find(
      product => product.id === productId
    );
  }

  getLineTotal(index: number): number {
    const control = this.lines.at(index);

    const productId =
      Number(control.get('productId')?.value);

    const quantity =
      Number(control.get('quantity')?.value) || 0;

    const discount =
      Number(
        control.get('discountPercentage')?.value
      ) || 0;

    const product = this.getProduct(productId);

    if (!product) {
      return 0;
    }

    const gross = product.price * quantity;
    const discountAmount = gross * discount / 100;

    return gross - discountAmount;
  }

  save(): void {
    this.error.set(null);
    this.form.markAllAsTouched();

    if (this.form.invalid || this.lines.length === 0) {
      return;
    }

    const value = this.form.getRawValue();

    const request: CreateOrderRequest = {
      customerId: value.customerId,
      notes: this.normalizeOptional(value.notes),
      lines: value.lines.map(line => ({
        productId: Number(line['productId']),
        quantity: Number(line['quantity']),
        discountPercentage: Number(
          line['discountPercentage']
        )
      }))
    };

    this.saving.set(true);

    this.orderService.createOrder(request).subscribe({
      next: order => {
        void this.router.navigate([
          '/orders',
          order.id
        ]);
      },
      error: error => {
        this.handleSaveError(error);
      }
    });
  }

  private handleSaveError(
    error: HttpErrorResponse
  ): void {
    console.error('Errore creazione ordine', error);

    if (error.status === 400) {
      this.formValidationService.applyBackendErrors(
        this.form,
        error.error as ValidationProblemDetails
      );

      this.error.set('Controlla i dati inseriti.');
    } else if (error.status === 409) {
      this.error.set(
        error.error?.detail ??
        'Impossibile creare l’ordine.'
      );
    } else {
      this.error.set(
        'Si è verificato un errore durante il salvataggio.'
      );
    }

    this.saving.set(false);
  }

  private normalizeOptional(
    value: string
  ): string | null {
    const normalized = value.trim();
    return normalized.length > 0 ? normalized : null;
  }

  private setupAutomaticCalculation(): void {
    this.lines.valueChanges
      .pipe(
        debounceTime(350),

        map(lines =>
          lines.map(line => ({
            productId: Number(line.productId),
            quantity: Number(line.quantity),
            discountPercentage: Number(
              line.discountPercentage
            )
          }))
        ),

        filter(lines =>
          lines.length > 0 &&
          lines.every(line =>
            line.productId > 0 &&
            line.quantity > 0 &&
            line.discountPercentage >= 0 &&
            line.discountPercentage <= 100
          )
        ),

        distinctUntilChanged(
          (previous, current) =>
            JSON.stringify(previous) ===
            JSON.stringify(current)
        ),

        switchMap(lines => {
          this.calculating.set(true);
          this.calculationError.set(null);

          return this.orderService
            .calculateOrder({ lines })
            .pipe(
              catchError(error => {
                console.error(
                  'Errore calcolo ordine',
                  error
                );

                this.calculation.set(null);
                this.calculating.set(false);

                this.calculationError.set(
                  error.error?.detail ??
                  'Impossibile calcolare il totale dell’ordine.'
                );

                return of(null);
              })
            );
        }),

        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: calculation => {
          if (calculation) {
            this.calculation.set(calculation);
          }

          this.calculating.set(false);
        },
        error: error => {
          console.error(
            'Errore calcolo ordine',
            error
          );

          this.calculation.set(null);
          this.calculating.set(false);

          this.calculationError.set(
            error.error?.detail ??
            'Impossibile calcolare il totale dell’ordine.'
          );
        }
      });
  }
}
