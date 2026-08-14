export interface Product {
  id: number;
  code: string;
  name: string;
  description: string | null;
  price: number;
  stockQuantity: number;
  categoryId: number;
  categoryName: string;
  vatPercentage: number;
  isActive: boolean;
  createdAtUtc: string;
}
