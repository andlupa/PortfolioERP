export interface PurchaseOrderLineRequest {
  productId: number;
  quantity: number;
  unitPrice: number;
  discountPercentage: number;
  vatPercentage: number;
}
