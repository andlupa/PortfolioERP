import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { AuthService } from '../../../auth/services/auth.service';
import { PurchaseOrder } from '../../models/purchase-order';
import { PurchaseOrderStatus } from '../../models/purchase-order-status';
import { PurchaseOrderService } from '../../services/purchase-order.service';

@Component({
  selector: 'app-purchase-order-detail-page',
  standalone: true,
  imports: [
    RouterLink,
    DatePipe,
    DecimalPipe
  ],
  templateUrl: './purchase-order-detail-page.html',
  styleUrl: './purchase-order-detail-page.scss'
})
export class PurchaseOrderDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly purchaseOrderService =
    inject(PurchaseOrderService);

  readonly authService = inject(AuthService);

  readonly order = signal<PurchaseOrder | null>(null);
  readonly loading = signal(true);
  readonly processing = signal(false);
  readonly error = signal<string | null>(null);

  readonly PurchaseOrderStatus = PurchaseOrderStatus;

  ngOnInit(): void {
    const id = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!id) {
      this.error.set('Ordine non valido.');
      this.loading.set(false);
      return;
    }

    this.loadOrder(id);
  }

  private loadOrder(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.purchaseOrderService
      .getById(id)
      .subscribe({
        next: order => {
          this.order.set(order);
          this.loading.set(false);
        },
        error: error => {
          console.error(
            'Errore caricamento ordine',
            error
          );

          this.error.set(
            'Impossibile caricare l\'ordine.'
          );

          this.loading.set(false);
        }
      });
  }

  markAsOrdered(): void {
    const order = this.order();

    if (!order) {
      return;
    }

    this.executeAction(
      () =>
        this.purchaseOrderService
          .markAsOrdered(order.id)
    );
  }

  receive(): void {
    const order = this.order();

    if (!order) {
      return;
    }

    this.executeAction(
      () =>
        this.purchaseOrderService
          .receive(order.id)
    );
  }

  cancel(): void {
    const order = this.order();

    if (!order) {
      return;
    }

    this.executeAction(
      () =>
        this.purchaseOrderService
          .cancel(order.id)
    );
  }

  getStatusLabel(
    status: PurchaseOrderStatus
  ): string {
    switch (status) {
      case PurchaseOrderStatus.Draft:
        return 'Bozza';

      case PurchaseOrderStatus.Ordered:
        return 'Ordinato';

      case PurchaseOrderStatus.Received:
        return 'Ricevuto';

      case PurchaseOrderStatus.Cancelled:
        return 'Annullato';

      default:
        return 'Sconosciuto';
    }
  }

  getStatusClass(
    status: PurchaseOrderStatus
  ): string {
    switch (status) {
      case PurchaseOrderStatus.Draft:
        return 'text-bg-secondary';

      case PurchaseOrderStatus.Ordered:
        return 'text-bg-primary';

      case PurchaseOrderStatus.Received:
        return 'text-bg-success';

      case PurchaseOrderStatus.Cancelled:
        return 'text-bg-danger';

      default:
        return 'text-bg-secondary';
    }
  }

  private executeAction(
    action: () => ReturnType<
      PurchaseOrderService['markAsOrdered']
    >
  ): void {
    this.processing.set(true);
    this.error.set(null);

    action().subscribe({
      next: updatedOrder => {
        this.order.set(updatedOrder);
        this.processing.set(false);
      },

      error: error => {
        console.error(
          'Errore aggiornamento ordine',
          error
        );

        this.error.set(
          'Impossibile aggiornare lo stato dell\'ordine.'
        );

        this.processing.set(false);
      }
    });
  }
}
