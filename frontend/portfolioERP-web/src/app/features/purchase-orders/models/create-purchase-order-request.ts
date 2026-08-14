import { PurchaseOrderLineRequest } from './purchase-order-line-request';

export interface CreatePurchaseOrderRequest {
  supplierId: number;
  orderDate: string;
  notes: string | null;
  lines: PurchaseOrderLineRequest[];
}
