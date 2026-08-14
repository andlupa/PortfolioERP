export interface ProductQuery {
  search?: string;
  categoryId?: number;
  isActive?: boolean;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  descending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}
