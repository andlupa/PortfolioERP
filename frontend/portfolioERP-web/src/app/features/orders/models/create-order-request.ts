import { CreateOrderLineRequest } from './create-order-line-request';

export interface CreateOrderRequest {
  customerId: number;
  notes: string | null;
  lines: CreateOrderLineRequest[];
}
