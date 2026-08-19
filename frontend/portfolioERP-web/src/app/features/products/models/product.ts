export interface Product {
  id: number;
  code: string;
  name: string;
  description: string | null;
  price: number;
  vatPercentage: number;
  quantityOnHand: number;
  quantityReserved: number;
  availableQuantity: number;
  isActive: boolean;
  createdAtUtc: string;
  categoryId: number;
  categoryName: string;
}
