export interface InvoiceLine {
    id: number;
    productId: number;
    productCode: string;
    description: string;
    quantity: number;
    unitPrice: number;
    discountPercentage: number;
    discountAmount: number;
    netAmount: number;
    vatPercentage: number;
    vatAmount: number;
    totalAmount: number;
}

export interface Invoice {
    id: number;
    invoiceNumber: string;
    salesOrderId: number;
    customerId: number;
    customerName: string;
    invoiceDateUtc: string;
    status: number;
    subtotal: number;
    discountAmount: number;
    taxAmount: number;
    totalAmount: number;
    lines: InvoiceLine[];
}