import { OrderStatus } from './order-status';

export interface OrderQuery {
  search?: string;
  customerId?: number;
  status?: OrderStatus;
  dateFrom?: string;
  dateTo?: string;
  minTotal?: number;
  maxTotal?: number;
  sortBy?: string;
  descending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}
