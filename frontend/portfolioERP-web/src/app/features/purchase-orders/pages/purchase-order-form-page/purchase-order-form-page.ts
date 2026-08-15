import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Product } from '../../../products/models/product';
import { ProductService } from '../../../products/services/product.service';
import { Supplier } from '../../../suppliers/models/supplier';
import { SupplierService } from '../../../suppliers/services/supplier.service';
import { PurchaseOrderService } from '../../services/purchase-order.service';
import { PurchaseOrderLineRequest } from '../../models/purchase-order-line-request';

@Component({
  selector: 'app-purchase-order-form-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './purchase-order-form-page.html',
  styleUrl: './purchase-order-form-page.scss'
})
export class PurchaseOrderFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly purchaseOrderService =
    inject(PurchaseOrderService);
  private readonly supplierService =
    inject(SupplierService);
  private readonly productService =
    inject(ProductService);
  private readonly router = inject(Router);

  readonly suppliers = signal<Supplier[]>([]);
  readonly products = signal<Product[]>([]);

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    supplierId: [
      0,
      Validators.min(1)
    ],

    orderDate: [
      this.getToday(),
      Validators.required
    ],

    notes: [''],

    lines: this.fb.array([])
  });

  get lines(): FormArray {
    return this.form.controls.lines as FormArray;
  }

  ngOnInit(): void {
    this.loadData();
    this.addLine();
  }

  private loadData(): void {
    this.loading.set(true);

    this.supplierService
      .getSuppliers({
        isActive: true,
        pageNumber: 1,
        pageSize: 100
      })
      .subscribe({
        next: response => {
          this.suppliers.set(response.items);
          this.loading.set(false);
        },
        error: () => {
          this.error.set(
            'Impossibile caricare i fornitori.'
          );
          this.loading.set(false);
        }
      });

    this.productService
      .getProducts({
        isActive: true,
        pageNumber: 1,
        pageSize: 100
      })
      .subscribe({
        next: response => {
          this.products.set(response.items);
        },
        error: () => {
          this.error.set(
            'Impossibile caricare i prodotti.'
          );
        }
      });
  }

  addLine(): void {
    const line = this.fb.nonNullable.group({
      productId: [
        0,
        Validators.min(1)
      ],

      quantity: [
        1,
        Validators.min(1)
      ],

      unitPrice: [
        0,
        Validators.min(0)
      ],

      discountPercentage: [
        0,
        [
          Validators.min(0),
          Validators.max(100)
        ]
      ],

      vatPercentage: [
        22,
        [
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

  lineTotal(index: number): number {
    const line = this.lines.at(index).getRawValue();

    const gross =
      line.quantity * line.unitPrice;

    const discount =
      gross * line.discountPercentage / 100;

    const net = gross - discount;

    const vat =
      net * line.vatPercentage / 100;

    return net + vat;
  }

  orderTotal(): number {
    return this.lines.controls.reduce(
      (total, _, index) =>
        total + this.lineTotal(index),
      0
    );
  }

  save(): void {
    this.error.set(null);
    this.form.markAllAsTouched();

    if (this.form.invalid || this.lines.length === 0) {
      return;
    }

    this.saving.set(true);

    const value = this.form.getRawValue();

    const rawLines =
      value.lines as PurchaseOrderLineRequest[];

    const lines: PurchaseOrderLineRequest[] =
      rawLines.map(line => ({
        productId: Number(line.productId),
        quantity: Number(line.quantity),
        unitPrice: Number(line.unitPrice),
        discountPercentage: Number(
          line.discountPercentage
        ),
        vatPercentage: Number(
          line.vatPercentage
        )
      }));

    this.purchaseOrderService
      .create({
        supplierId: value.supplierId,

        orderDate:
          `${value.orderDate}T00:00:00`,

        notes:
          value.notes.trim() || null,

        lines
      })
      .subscribe({
        next: order => {
          void this.router.navigate([
            '/purchase-orders',
            order.id
          ]);
        },

        error: error => {
          console.error(
            'Errore creazione ordine',
            error
          );

          this.error.set(
            'Impossibile creare l\'ordine di acquisto.'
          );

          this.saving.set(false);
        }
      });
  }

  private getToday(): string {
    const date = new Date();

    return [
      date.getFullYear(),
      String(date.getMonth() + 1).padStart(2, '0'),
      String(date.getDate()).padStart(2, '0')
    ].join('-');
  }
}
