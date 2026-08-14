export interface OrderCalculationLineResponse {
  productId: number;
  unitPrice: number;
  quantity: number;
  grossAmount: number;
  discountPercentage: number;
  discountAmount: number;
  netAmount: number;
  vatPercentage: number;
  vatAmount: number;
  totalAmount: number;
}

export interface OrderCalculationResponse {
  lines: OrderCalculationLineResponse[];
  subtotal: number;
  discountAmount: number;
  netAmount: number;
  taxAmount: number;
  totalAmount: number;
}
