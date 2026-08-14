export interface CreateSupplierRequest {
  supplierCode: string;
  companyName: string;
  contactName: string | null;
  vatNumber: string | null;
  taxCode: string | null;
  email: string;
  phone: string | null;
  address: string | null;
  city: string | null;
  province: string | null;
  postalCode: string | null;
  country: string;
}
