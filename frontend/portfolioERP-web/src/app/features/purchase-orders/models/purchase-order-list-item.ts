import { PurchaseOrderStatus } from './purchase-order-status';

export interface PurchaseOrderListItem {
  id: number;
  orderNumber: string;
  supplierId: number;
  supplierCompanyName: string;
  orderDate: string;
  status: PurchaseOrderStatus;
  totalAmount: number;
}
