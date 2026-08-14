export interface OrderLineResponse {
  id: number;
  productId: number;
  productCode: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  discountPercentage: number;
  discountAmount: number;
  netAmount: number;
  vatPercentage: number;
  vatAmount: number;
  totalAmount: number;
}
