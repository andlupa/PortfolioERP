import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import { SupplierService } from '../../services/supplier.service';

@Component({
  selector: 'app-supplier-form-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './supplier-form-page.html',
  styleUrl: './supplier-form-page.scss'
})
export class SupplierFormPage implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly supplierService = inject(SupplierService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly saving = signal(false);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly supplierId = signal<number | null>(null);

  readonly form = this.formBuilder.nonNullable.group({
    supplierCode: [
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

    contactName: [''],

    vatNumber: [''],

    taxCode: [''],

    email: [
      '',
      [
        Validators.required,
        Validators.email,
        Validators.maxLength(200)
      ]
    ],

    phone: [''],

    address: [''],

    city: [''],

    province: [''],

    postalCode: [''],

    country: [
      'Italy',
      Validators.required
    ],

    isActive: [true]
  });

  get isEditMode(): boolean {
    return this.supplierId() !== null;
  }

  get pageTitle(): string {
    return this.isEditMode
      ? 'Modifica fornitore'
      : 'Nuovo fornitore';
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id !== null) {
      const supplierId = Number(id);

      if (!Number.isNaN(supplierId)) {
        this.supplierId.set(supplierId);
        this.loadSupplier(supplierId);
      }
    }
  }

  private loadSupplier(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.supplierService
      .getById(id)
      .subscribe({
        next: supplier => {
          this.form.patchValue({
            supplierCode: supplier.supplierCode,
            companyName: supplier.companyName,
            contactName: supplier.contactName ?? '',
            vatNumber: supplier.vatNumber ?? '',
            taxCode: supplier.taxCode ?? '',
            email: supplier.email,
            phone: supplier.phone ?? '',
            address: supplier.address ?? '',
            city: supplier.city ?? '',
            province: supplier.province ?? '',
            postalCode: supplier.postalCode ?? '',
            country: supplier.country,
            isActive: supplier.isActive
          });

          this.loading.set(false);
        },

        error: error => {
          console.error(
            'Errore caricamento fornitore',
            error
          );

          this.error.set(
            'Impossibile caricare il fornitore.'
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

    const request = {
      supplierCode: value.supplierCode,
      companyName: value.companyName,
      contactName: value.contactName || null,
      vatNumber: value.vatNumber || null,
      taxCode: value.taxCode || null,
      email: value.email,
      phone: value.phone || null,
      address: value.address || null,
      city: value.city || null,
      province: value.province || null,
      postalCode: value.postalCode || null,
      country: value.country
    };

    const id = this.supplierId();

    const operation = id === null
      ? this.supplierService.create(request)
      : this.supplierService.update(
          id,
          {
            ...request,
            isActive: value.isActive
          }
        );

    operation.subscribe({
      next: () => {
        void this.router.navigate(['/suppliers']);
      },

      error: (error: HttpErrorResponse) => {
        console.error(
          'Errore salvataggio fornitore',
          error
        );

        if (
          error.error?.message ===
          'Supplier code already exists.'
        ) {
          this.error.set(
            'Esiste già un fornitore con questo codice.'
          );
        } else {
          this.error.set(
            'Impossibile salvare il fornitore.'
          );
        }

        this.saving.set(false);
      }
    });
  }
}
