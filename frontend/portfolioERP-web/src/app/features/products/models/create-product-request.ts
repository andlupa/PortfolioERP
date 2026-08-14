export interface CreateProductRequest {
  code: string;
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  categoryId: number;
  vatPercentage: number;
}
