import type { Invoice } from '../models/invoice';
import { getToken, removeAuth } from '../auth/tokenStorage';

const API_URL =
    import.meta.env.VITE_INVOICE_API_URL;

async function authenticatedFetch(
    url: string
): Promise<Response> {
    const token = getToken();

    const response = await fetch(url, {
        headers: token
            ? {
                Authorization: `Bearer ${token}`
            }
            : {}
    });

    if (response.status === 401) {
        removeAuth();

        window.location.href = '/login';
    }

    return response;
}

export async function getInvoices(): Promise<Invoice[]> {
    const response = await authenticatedFetch(
        `${API_URL}/api/invoices`
    );

    if (!response.ok) {
        throw new Error(
            'Impossibile caricare le fatture.'
        );
    }

    return response.json();
}

export async function getInvoiceById(
    id: number
): Promise<Invoice> {
    const response = await authenticatedFetch(
        `${API_URL}/api/invoices/${id}`
    );

    if (!response.ok) {
        throw new Error(
            'Impossibile caricare la fattura.'
        );
    }

    return response.json();
}
