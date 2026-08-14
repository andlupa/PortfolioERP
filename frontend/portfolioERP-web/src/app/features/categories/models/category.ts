export interface Category {
  id: number;
  name: string;
  description: string | null;
  isActive: boolean;
  createdAtUtc: string;
}
