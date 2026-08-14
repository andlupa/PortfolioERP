import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

import { PurchaseOrderListItem } from '../../models/purchase-order-list-item';
import { PurchaseOrderStatus } from '../../models/purchase-order-status';
import { PurchaseOrderService } from '../../services/purchase-order.service';

@Component({
  selector: 'app-purchase-order-list-page',
  standalone: true,
  imports: [
    RouterLink,
    DatePipe,
    DecimalPipe
  ],
  templateUrl: './purchase-order-list-page.html',
  styleUrl: './purchase-order-list-page.scss'
})
export class PurchaseOrderListPage implements OnInit {
  private readonly purchaseOrderService =
    inject(PurchaseOrderService);

  readonly orders =
    signal<PurchaseOrderListItem[]>([]);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly PurchaseOrderStatus =
    PurchaseOrderStatus;

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading.set(true);
    this.error.set(null);

    this.purchaseOrderService
      .getAll()
      .subscribe({
        next: orders => {
          this.orders.set(orders);
          this.loading.set(false);
        },

        error: error => {
          console.error(
            'Errore caricamento ordini di acquisto',
            error
          );

          this.error.set(
            'Impossibile caricare gli ordini di acquisto.'
          );

          this.loading.set(false);
        }
      });
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
}
