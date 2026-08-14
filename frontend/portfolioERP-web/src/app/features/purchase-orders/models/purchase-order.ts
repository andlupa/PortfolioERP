import { PurchaseOrderLine } from './purchase-order-line';
import { PurchaseOrderStatus } from './purchase-order-status';

export interface PurchaseOrder {
  id: number;
  orderNumber: string;

  supplierId: number;
  supplierCode: string;
  supplierCompanyName: string;

  orderDate: string;
  status: PurchaseOrderStatus;

  netAmount: number;
  vatAmount: number;
  totalAmount: number;

  notes: string | null;

  createdAtUtc: string;
  updatedAtUtc: string | null;

  lines: PurchaseOrderLine[];
}
