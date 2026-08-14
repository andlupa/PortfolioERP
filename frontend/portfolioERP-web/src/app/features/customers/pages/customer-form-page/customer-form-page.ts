import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { CreateCustomerRequest } from '../../models/create-customer-request';
import { UpdateCustomerRequest } from '../../models/update-customer-request';
import { CustomerService } from '../../services/customer.service';
import { FormValidationService } from '../../../../shared/services/form-validation.service';
import { ValidationProblemDetails } from '../../../../shared/models/validation-problem-details';

@Component({
  selector: 'app-customer-form-page',
  standalone: true,
  imports: [ ReactiveFormsModule, RouterLink ],
  templateUrl: './customer-form-page.html',
  styleUrl: './customer-form-page.scss'
})
export class CustomerFormPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly customerService = inject(CustomerService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly formValidationService = inject(FormValidationService);

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  customerId: number | null = null;

  readonly form = this.formBuilder.nonNullable.group({
    customerCode: [
      '',
      [
        Validators.required,
        Validators.maxLength(30)
      ]
    ],
    companyName: [
      '',
      [
        Validators.required,
        Validators.maxLength(200)
      ]
    ],
    firstName: [
      '',
      Validators.maxLength(100)
    ],
    lastName: [
      '',
      Validators.maxLength(100)
    ],
    taxCode: [
      '',
      Validators.maxLength(16)
    ],
    vatNumber: [
      '',
      Validators.maxLength(20)
    ],
    email: [
      '',
      [
        Validators.required,
        Validators.email,
        Validators.maxLength(200)
      ]
    ],
    phone: [
      '',
      Validators.maxLength(30)
    ],
    address: [
      '',
      Validators.maxLength(250)
    ],
    city: [
      '',
      Validators.maxLength(100)
    ],
    province: [
      '',
      Validators.maxLength(10)
    ],
    postalCode: [
      '',
      Validators.maxLength(15)
    ],
    country: [
      'Italy',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],
    isActive: [true]
  });

  get isEditMode(): boolean {
    return this.customerId !== null;
  }

  get pageTitle(): string {
    return this.isEditMode
      ? 'Modifica cliente'
      : 'Nuovo cliente';
  }

  ngOnInit(): void {
    this.formValidationService
      .clearBackendErrorsOnValueChanges(this.form);

    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (Number.isInteger(id) && id > 0) {
      this.customerId = id;
      this.loadCustomer(id);
    }  
  }

  loadCustomer(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.customerService.getCustomerById(id).subscribe({
      next: customer => {
        this.form.patchValue({
          customerCode: customer.customerCode,
          companyName: customer.companyName,
          firstName: customer.firstName ?? '',
          lastName: customer.lastName ?? '',
          taxCode: customer.taxCode ?? '',
          vatNumber: customer.vatNumber ?? '',
          email: customer.email,
          phone: customer.phone ?? '',
          address: customer.address ?? '',
          city: customer.city ?? '',
          province: customer.province ?? '',
          postalCode: customer.postalCode ?? '',
          country: customer.country,
          isActive: customer.isActive
        });

        this.loading.set(false);
      },
      error: error => {
        console.error('Errore caricamento cliente', error);

        this.error.set(
          error.status === 404
            ? 'Cliente non trovato.'
            : 'Impossibile caricare il cliente.'
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

    if (this.customerId === null) {
      const request: CreateCustomerRequest = {
        customerCode: value.customerCode.trim(),
        companyName: value.companyName.trim(),
        firstName: this.normalizeOptional(value.firstName),
        lastName: this.normalizeOptional(value.lastName),
        taxCode: this.normalizeOptional(value.taxCode),
        vatNumber: this.normalizeOptional(value.vatNumber),
        email: value.email.trim(),
        phone: this.normalizeOptional(value.phone),
        address: this.normalizeOptional(value.address),
        city: this.normalizeOptional(value.city),
        province: this.normalizeOptional(value.province),
        postalCode: this.normalizeOptional(value.postalCode),
        country: value.country.trim()
      };

      this.customerService.createCustomer(request).subscribe({
        next: () => {
          void this.router.navigate(['/customers']);
        },
        error: error => this.handleSaveError(error)
      });

      return;
    }

    const request: UpdateCustomerRequest = {
      customerCode: value.customerCode.trim(),
      companyName: value.companyName.trim(),
      firstName: this.normalizeOptional(value.firstName),
      lastName: this.normalizeOptional(value.lastName),
      taxCode: this.normalizeOptional(value.taxCode),
      vatNumber: this.normalizeOptional(value.vatNumber),
      email: value.email.trim(),
      phone: this.normalizeOptional(value.phone),
      address: this.normalizeOptional(value.address),
      city: this.normalizeOptional(value.city),
      province: this.normalizeOptional(value.province),
      postalCode: this.normalizeOptional(value.postalCode),
      country: value.country.trim(),
      isActive: value.isActive
    };

    this.customerService
      .updateCustomer(this.customerId, request)
      .subscribe({
        next: () => {
          void this.router.navigate(['/customers']);
        },
        error: error => this.handleSaveError(error)
      });
  }

  private handleSaveError(error: HttpErrorResponse): void {
    console.error('Errore salvataggio cliente', error);

    if (error.status === 400) {
      this.formValidationService.applyBackendErrors(
        this.form,
        error.error as ValidationProblemDetails
      );
      this.error.set('Controlla i dati inseriti.');
    } else if (error.status === 409) {
      this.error.set(
        error.error?.detail ??
        'Esiste già un cliente con gli stessi dati identificativi.'
      );
    } else {
      this.error.set('Impossibile salvare il cliente.');
    }

    this.saving.set(false);
  }

  private normalizeOptional(value: string): string | null {
    const normalized = value.trim();
    return normalized.length > 0 ? normalized : null;
  }
}
