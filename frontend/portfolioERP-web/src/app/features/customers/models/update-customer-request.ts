import { CreateCustomerRequest } from '../models/create-customer-request';

export interface UpdateCustomerRequest
  extends CreateCustomerRequest {
  isActive: boolean;
}
