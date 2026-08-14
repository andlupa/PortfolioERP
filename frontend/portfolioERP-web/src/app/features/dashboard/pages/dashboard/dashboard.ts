import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  DestroyRef,
  ElementRef,
  inject,
  OnInit,
  signal,
  ViewChild
} from '@angular/core';
import { Router } from '@angular/router';

import { DashboardResponse } from '../../models/dashboard-response';
import { DashboardService } from '../../services/dashboard.service';
import {
  getOrderStatusLabel,
  OrderStatus
} from '../../../orders/models/order-status';

import {
  Chart,
  ChartConfiguration,
  registerables
} from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit, AfterViewInit {
  private readonly dashboardService = inject(DashboardService);
  private readonly router = inject(Router);

  @ViewChild('orderStatusChart')
  private orderStatusChartCanvas?: ElementRef<HTMLCanvasElement>;

  private orderStatusChart?: Chart<'doughnut'>;
  private viewInitialized = false;

  @ViewChild('monthlyRevenueChart')
  private monthlyRevenueChartCanvas?: ElementRef<HTMLCanvasElement>;

  private monthlyRevenueChart?: Chart<'line'>;

  readonly dashboard = signal<DashboardResponse | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly OrderStatus = OrderStatus;
  
  ngOnInit(): void {
    this.loadDashboard();
  }

  ngAfterViewInit(): void {
    this.viewInitialized = true;

    this.createOrderStatusChart();
    this.createMonthlyRevenueChart();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.error.set(null);

    this.dashboardService.getDashboard().subscribe({
      next: response => {
        this.dashboard.set(response);
        this.loading.set(false);

        setTimeout(() => {
          this.createOrderStatusChart();
          this.createMonthlyRevenueChart();
        });
      },

      error: error => {
        console.error('Errore caricamento dashboard', error);

        this.error.set(
          'Impossibile caricare la dashboard.'
        );

        this.loading.set(false);
      }
    });
  }

  private createMonthlyRevenueChart(): void {
    if (!this.viewInitialized) {
      return;
    }

    const canvas =
      this.monthlyRevenueChartCanvas?.nativeElement;

    const data =
      this.dashboard()?.monthlyRevenue;

    if (!canvas || !data || data.length === 0) {
      return;
    }

    this.monthlyRevenueChart?.destroy();

    const monthFormatter =
      new Intl.DateTimeFormat('it-IT', {
        month: 'short',
        year: 'numeric'
      });

    const config: ChartConfiguration<'line'> = {
      type: 'line',

      data: {
        labels: data.map(item =>
          monthFormatter.format(
            new Date(item.year, item.month - 1, 1)
          )
        ),

        datasets: [
          {
            label: 'Fatturato',
            data: data.map(item => item.revenue),
            tension: 0.25,
            fill: false
          }
        ]
      },

      options: {
        responsive: true,
        maintainAspectRatio: false,

        plugins: {
          legend: {
            display: false
          },

          tooltip: {
            callbacks: {
              label: context => {
                const value =
                  Number(context.parsed.y) || 0;

                return new Intl.NumberFormat(
                  'it-IT',
                  {
                    style: 'currency',
                    currency: 'EUR'
                  }
                ).format(value);
              }
            }
          }
        },

        scales: {
          y: {
            beginAtZero: true,

            ticks: {
              callback: value =>
                new Intl.NumberFormat(
                  'it-IT',
                  {
                    style: 'currency',
                    currency: 'EUR',
                    maximumFractionDigits: 0
                  }
                ).format(Number(value))
            }
          }
        }
      }
    };

    this.monthlyRevenueChart =
      new Chart(canvas, config);
  }

  private createOrderStatusChart(): void {
    if (!this.viewInitialized) {
      return;
    }

    const canvas = this.orderStatusChartCanvas?.nativeElement;
    const data = this.dashboard()?.ordersByStatus;

    if (!canvas || !data || data.length === 0) {
      return;
    }

    this.orderStatusChart?.destroy();

    const config: ChartConfiguration<'doughnut'> = {
      type: 'doughnut',

      data: {
        labels: data.map(item =>
          this.getStatusLabel(item.status)
        ),

        datasets: [
          {
            data: data.map(item => item.count)
          }
        ]
      },

      options: {
        responsive: true,
        maintainAspectRatio: false,

        plugins: {
          legend: {
            position: 'bottom'
          }
        }
      }
    };

    this.orderStatusChart =
      new Chart(canvas, config);
  }

  openOrder(id: number): void {
    void this.router.navigate(['/orders', id]);
  }

  openProduct(id: number): void {
    void this.router.navigate(['/products', id, 'edit']);
  }

  openProducts(): void {
    void this.router.navigate(['/products']);
  }

  openCustomers(): void {
    void this.router.navigate(['/customers']);
  }

  openOrders(): void {
    void this.router.navigate(['/orders']);
  }

  getStatusLabel(status: OrderStatus): string {
    return getOrderStatusLabel(status);
  }

  getStatusClass(status: OrderStatus): string {
    switch (status) {
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
}
