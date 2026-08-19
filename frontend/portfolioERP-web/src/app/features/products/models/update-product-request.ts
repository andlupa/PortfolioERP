export interface UpdateProductRequest {
  code: string;
  name: string;
  description: string | null;
  price: number;
  categoryId: number;
  vatPercentage: number;
  isActive: boolean;
}
