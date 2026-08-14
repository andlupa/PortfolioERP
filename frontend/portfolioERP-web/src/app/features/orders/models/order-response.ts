import { OrderLineResponse } from './order-line-response';
import { OrderStatus } from './order-status';

export interface OrderResponse {
  id: number;
  orderNumber: string;
  orderDate: string;
  status: OrderStatus;
  customerId: number;
  customerName: string;
  notes: string | null;
  subtotal: number;
  discountAmount: number;
  taxAmount: number;
  totalAmount: number;
  createdAtUtc: string;
  updatedAtUtc: string | null;
  lines: OrderLineResponse[];
}
