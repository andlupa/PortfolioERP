import { OrderStatus } from './order-status';

export interface OrderListItem {
  id: number;
  orderNumber: string;
  orderDate: string;
  status: OrderStatus;
  customerId: number;
  customerName: string;
  lineCount: number;
  totalAmount: number;
}
