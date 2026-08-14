import { CommonModule } from '@angular/common';
import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import {
  AppToast,
  ToastType
} from '../../../../shared/components/app-toast/app-toast';

import {
  ConfirmDialog
} from '../../../../shared/components/confirm-dialog/confirm-dialog';

import {
  OrderResponse
} from '../../models/order-response';

import {
  getOrderStatusLabel,
  OrderStatus
} from '../../models/order-status';

import {
  OrderService
} from '../../services/order.service';

type PendingOrderAction =
  'confirm' |
  'cancel' |
  null;

@Component({
  selector: 'app-order-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ConfirmDialog,
    AppToast
  ],
  templateUrl: './order-detail-page.html',
  styleUrl: './order-detail-page.scss'
})
export class OrderDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly orderService = inject(OrderService);

  readonly order = signal<OrderResponse | null>(null);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly pendingAction =
    signal<PendingOrderAction>(null);

  readonly processing = signal(false);

  readonly toastVisible = signal(false);
  readonly toastMessage = signal('');
  readonly toastType =
    signal<ToastType>('success');

  readonly OrderStatus = OrderStatus;

  private toastTimer:
    ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    const id = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!Number.isInteger(id) || id <= 0) {
      this.error.set('Identificativo ordine non valido.');
      return;
    }

    this.loadOrder(id);
  }

  get statusLabel(): string {
    const order = this.order();

    return order
      ? getOrderStatusLabel(order.status)
      : '';
  }

  get statusClass(): string {
    const order = this.order();

    if (!order) {
      return 'text-bg-secondary';
    }

    switch (order.status) {
      case OrderStatus.Draft:
        return 'text-bg-secondary';

      case OrderStatus.Confirmed:
        return 'text-bg-primary';

      case OrderStatus.Processing:
        return 'text-bg-warning';

      case OrderStatus.Shipped:
        return 'text-bg-info';

      case OrderStatus.Completed:
        return 'text-bg-success';

      case OrderStatus.Cancelled:
        return 'text-bg-danger';

      default:
        return 'text-bg-secondary';
    }
  }

  get canConfirm(): boolean {
    return this.order()?.status === OrderStatus.Draft;
  }

  get canCancel(): boolean {
    const status = this.order()?.status;

    return status !== undefined &&
      status !== OrderStatus.Cancelled &&
      status !== OrderStatus.Shipped &&
      status !== OrderStatus.Completed;
  }

  get dialogOpen(): boolean {
    return this.pendingAction() !== null;
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

  get dialogDanger(): boolean {
    return this.pendingAction() === 'cancel';
  }

  get dialogMessage(): string {
    const order = this.order();

    if (!order) {
      return '';
    }

    if (this.pendingAction() === 'confirm') {
      return `Vuoi confermare l’ordine ${order.orderNumber}?`;
    }

    return `Vuoi annullare l’ordine ${order.orderNumber}? Le quantità saranno restituite al magazzino.`;
  }

  loadOrder(id: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.orderService.getOrderById(id).subscribe({
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
          error.status === 404
            ? 'Ordine non trovato.'
            : 'Impossibile caricare l’ordine.'
        );

        this.loading.set(false);
      }
    });
  }

  requestConfirm(): void {
    this.pendingAction.set('confirm');
  }

  requestCancel(): void {
    this.pendingAction.set('cancel');
  }

  closeDialog(): void {
    if (!this.processing()) {
      this.pendingAction.set(null);
    }
  }

  executeAction(): void {
    const order = this.order();
    const action = this.pendingAction();

    if (!order || !action) {
      return;
    }

    if (action === 'confirm') {
      this.confirmOrder(order.id);
    } else {
      this.cancelOrder(order.id);
    }
  }

  private confirmOrder(id: number): void {
    this.processing.set(true);

    this.orderService.confirmOrder(id).subscribe({
      next: () => {
        this.processing.set(false);
        this.pendingAction.set(null);

        this.showToast(
          'Ordine confermato correttamente.',
          'success'
        );

        this.loadOrder(id);
      },

      error: error => {
        console.error(
          'Errore conferma ordine',
          error
        );

        this.processing.set(false);
        this.pendingAction.set(null);

        this.showToast(
          error.error?.detail ??
          'Impossibile confermare l’ordine.',
          'danger'
        );
      }
    });
  }

  private cancelOrder(id: number): void {
    this.processing.set(true);

    this.orderService.cancelOrder(id).subscribe({
      next: () => {
        this.processing.set(false);
        this.pendingAction.set(null);

        this.showToast(
          'Ordine annullato correttamente.',
          'success'
        );

        this.loadOrder(id);
      },

      error: error => {
        console.error(
          'Errore annullamento ordine',
          error
        );

        this.processing.set(false);
        this.pendingAction.set(null);

        this.showToast(
          error.error?.detail ??
          'Impossibile annullare l’ordine.',
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
