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
  OrderFilter,
  OrderFilters
} from '../../components/order-filter/order-filter';
import {
  OrderPagination
} from '../../components/order-pagination/order-pagination';
import {
  OrderTable
} from '../../components/order-table/order-table';
import {
  OrderToolbar
} from '../../components/order-toolbar/order-toolbar';
import { OrderListItem } from '../../models/order-list-item';
import { OrderStatus } from '../../models/order-status';
import { OrderService } from '../../services/order.service';

type PendingOrderAction = 'confirm' | 'cancel' | null;

@Component({
  selector: 'app-order-list-page',
  standalone: true,
  imports: [
    OrderToolbar,
    OrderFilter,
    OrderTable,
    OrderPagination,
    ConfirmDialog,
    AppToast
  ],
  templateUrl: './order-list-page.html',
  styleUrl: './order-list-page.scss'
})
export class OrderListPage implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly router = inject(Router);

  readonly orders = signal<OrderListItem[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly selectedOrder = signal<OrderListItem | null>(null);
  readonly pendingAction = signal<PendingOrderAction>(null);
  readonly processingOrderId = signal<number | null>(null);

  readonly toastVisible = signal(false);
  readonly toastMessage = signal('');
  readonly toastType = signal<ToastType>('success');

  search = '';
  status: OrderStatus | null = null;
  dateFrom = '';
  dateTo = '';

  sortBy = 'orderDate';
  descending = true;

  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;

  private toastTimer:
    ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.loadOrders();
  }

  get dialogOpen(): boolean {
    return (
      this.selectedOrder() !== null &&
      this.pendingAction() !== null
    );
  }

  get dialogTitle(): string {
    return this.pendingAction() === 'confirm'
      ? 'Conferma ordine'
      : 'Annulla ordine';
  }

  get dialogConfirmText(): string {
    return this.pendingAction() === 'confirm'
      ? 'Conferma ordine'
      : 'Annulla ordine';
  }

  get dialogMessage(): string {
    const order = this.selectedOrder();

    if (!order) {
      return '';
    }

    if (this.pendingAction() === 'confirm') {
      return `Vuoi confermare l’ordine ${order.orderNumber} di ${order.customerName}?`;
    }

    return `Vuoi annullare l’ordine ${order.orderNumber}? Le quantità saranno restituite al magazzino.`;
  }

  get dialogDanger(): boolean {
    return this.pendingAction() === 'cancel';
  }

  loadOrders(): void {
    this.loading.set(true);
    this.error.set(null);

    this.orderService.getOrders({
      search: this.search,
      status: this.status ?? undefined,
      dateFrom: this.dateFrom || undefined,
      dateTo: this.dateTo || undefined,
      sortBy: this.sortBy,
      descending: this.descending,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    }).subscribe({
      next: response => {
        this.orders.set(response.items);
        this.totalItems = response.totalItems;
        this.totalPages = response.totalPages;

        if (
          this.totalPages > 0 &&
          this.pageNumber > this.totalPages
        ) {
          this.pageNumber = this.totalPages;
          this.loadOrders();
          return;
        }

        this.loading.set(false);
      },
      error: error => {
        console.error('Errore caricamento ordini', error);
        this.error.set('Impossibile caricare gli ordini.');
        this.loading.set(false);
      }
    });
  }

  applyFilters(filters: OrderFilters): void {
    this.search = filters.search;
    this.status = filters.status;
    this.dateFrom = filters.dateFrom;
    this.dateTo = filters.dateTo;
    this.pageNumber = 1;

    this.loadOrders();
  }

  resetFilters(): void {
    this.search = '';
    this.status = null;
    this.dateFrom = '';
    this.dateTo = '';
    this.pageNumber = 1;

    this.loadOrders();
  }

  changeSorting(column: string): void {
    if (this.sortBy === column) {
      this.descending = !this.descending;
    } else {
      this.sortBy = column;
      this.descending = false;
    }

    this.pageNumber = 1;
    this.loadOrders();
  }

  changePage(pageNumber: number): void {
    this.pageNumber = pageNumber;
    this.loadOrders();
  }

  createOrder(): void {
    void this.router.navigate(['/orders/new']);
  }

  openDetails(orderId: number): void {
    void this.router.navigate(['/orders', orderId]);
  }

  requestConfirmation(order: OrderListItem): void {
    this.selectedOrder.set(order);
    this.pendingAction.set('confirm');
  }

  requestCancellation(order: OrderListItem): void {
    this.selectedOrder.set(order);
    this.pendingAction.set('cancel');
  }

  closeDialog(): void {
    if (this.processingOrderId() !== null) {
      return;
    }

    this.selectedOrder.set(null);
    this.pendingAction.set(null);
  }

  executePendingAction(): void {
    const order = this.selectedOrder();
    const action = this.pendingAction();

    if (!order || !action) {
      return;
    }

    if (action === 'confirm') {
      this.confirmOrder(order);
      return;
    }

    this.cancelOrder(order);
  }

  closeToast(): void {
    this.toastVisible.set(false);

    if (this.toastTimer) {
      clearTimeout(this.toastTimer);
      this.toastTimer = null;
    }
  }

  private confirmOrder(order: OrderListItem): void {
    this.processingOrderId.set(order.id);

    this.orderService.confirmOrder(order.id).subscribe({
      next: () => {
        this.processingOrderId.set(null);
        this.selectedOrder.set(null);
        this.pendingAction.set(null);

        this.showToast(
          `L’ordine ${order.orderNumber} è stato confermato.`,
          'success'
        );

        this.loadOrders();
      },
      error: error => {
        console.error('Errore conferma ordine', error);

        this.processingOrderId.set(null);
        this.selectedOrder.set(null);
        this.pendingAction.set(null);

        this.showToast(
          this.getOperationError(
            error,
            'Impossibile confermare l’ordine.'
          ),
          'danger'
        );
      }
    });
  }

  private cancelOrder(order: OrderListItem): void {
    this.processingOrderId.set(order.id);

    this.orderService.cancelOrder(order.id).subscribe({
      next: () => {
        this.processingOrderId.set(null);
        this.selectedOrder.set(null);
        this.pendingAction.set(null);

        this.showToast(
          `L’ordine ${order.orderNumber} è stato annullato.`,
          'success'
        );

        this.loadOrders();
      },
      error: error => {
        console.error('Errore annullamento ordine', error);

        this.processingOrderId.set(null);
        this.selectedOrder.set(null);
        this.pendingAction.set(null);

        this.showToast(
          this.getOperationError(
            error,
            'Impossibile annullare l’ordine.'
          ),
          'danger'
        );
      }
    });
  }

  private getOperationError(
    error: any,
    fallbackMessage: string
  ): string {
    if (error.status === 404) {
      return 'Ordine non trovato.';
    }

    if (error.status === 409) {
      return error.error?.detail ?? fallbackMessage;
    }

    return fallbackMessage;
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
