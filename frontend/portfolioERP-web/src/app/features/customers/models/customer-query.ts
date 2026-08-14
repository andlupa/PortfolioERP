export interface CustomerQuery {
  search?: string;
  city?: string;
  country?: string;
  isActive?: boolean;
  sortBy?: string;
  descending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}
