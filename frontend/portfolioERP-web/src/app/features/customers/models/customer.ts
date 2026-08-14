export interface Customer {
  id: number;
  customerCode: string;
  companyName: string;
  firstName: string | null;
  lastName: string | null;
  taxCode: string | null;
  vatNumber: string | null;
  email: string;
  phone: string | null;
  address: string | null;
  city: string | null;
  province: string | null;
  postalCode: string | null;
  country: string;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}
