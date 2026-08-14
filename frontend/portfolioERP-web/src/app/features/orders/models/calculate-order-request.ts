export interface CalculateOrderLineRequest {
  productId: number;
  quantity: number;
  discountPercentage: number;
}

export interface CalculateOrderRequest {
  lines: CalculateOrderLineRequest[];
}
