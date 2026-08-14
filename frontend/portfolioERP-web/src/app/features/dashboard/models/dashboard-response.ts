import { OrderStatus } from '../../orders/models/order-status';

export interface OrderStatusSummary {
  status: OrderStatus;
  count: number;
}

export interface RecentOrder {
  id: number;
  orderNumber: string;
  orderDate: string;
  customerName: string;
  status: OrderStatus;
  totalAmount: number;
}

export interface LowStockProduct {
  id: number;
  code: string;
  name: string;
  stockQuantity: number;
}

export interface DashboardResponse {
  activeProducts: number;
  activeCustomers: number;
  totalOrders: number;
  ordersThisMonth: number;
  revenueThisMonth: number;
  monthlyRevenue: MonthlyRevenue[];
  ordersByStatus: OrderStatusSummary[];
  recentOrders: RecentOrder[];
  lowStockProducts: LowStockProduct[];
}
export interface MonthlyRevenue {
  year: number;
  month: number;
  revenue: number;
}
