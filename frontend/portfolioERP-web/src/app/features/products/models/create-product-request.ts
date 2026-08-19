export interface CreateProductRequest {
  code: string;
  name: string;
  description: string | null;
  price: number;
  categoryId: number;
  vatPercentage: number;
}
