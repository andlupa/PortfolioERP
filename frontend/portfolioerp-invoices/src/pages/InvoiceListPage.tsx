import { useEffect, useState } from 'react';

import { getInvoices } from '../api/invoiceApi';
import { Link } from 'react-router-dom';

import type { Invoice } from '../models/invoice';

export default function InvoiceListPage() {
    const [invoices, setInvoices] =
        useState<Invoice[]>([]);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        getInvoices()
            .then(data => {
                setInvoices(data);
            })
            .catch(error => {
                console.error(error);

                setError(
                    'Impossibile caricare le fatture.'
                );
            })
            .finally(() => {
                setLoading(false);
            });
    }, []);

    if (loading) {
        return <p>Caricamento fatture...</p>;
    }

    if (error) {
        return <p>{error}</p>;
    }

    return (
        <>
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h1 className="h3 mb-1">Fatture</h1>

                    <p className="text-secondary mb-0">
                        Fatture generate in automatico dagli ordini spediti
                    </p>
                </div>

                <span className="badge text-bg-secondary fs-6">
                    {invoices.length} fatture
                </span>
            </div>

            <div className="card shadow-sm">
                <div className="card-body p-0">
                    <div className="table-responsive">

                        <table className="table table-hover align-middle mb-0">
                            <thead className="table-light">
                                <tr>
                                    <th>Numero</th>
                                    <th>Cliente</th>
                                    <th>Data</th>
                                    <th>Ordine</th>
                                    <th>Stato</th>
                                    <th className="text-end">
                                        Totale
                                    </th>
                                </tr>
                            </thead>

                            <tbody>
                                {invoices.map(invoice => (
                                    <tr key={invoice.id}>

                                        <td>
                                            <Link
                                                className="fw-semibold text-decoration-none"
                                                to={`/invoices/${invoice.id}`}
                                            >
                                                {invoice.invoiceNumber}
                                            </Link>
                                        </td>

                                        <td>
                                            {invoice.customerName}
                                        </td>

                                        <td>
                                            {new Date(
                                                invoice.invoiceDateUtc
                                            ).toLocaleDateString('it-IT')}
                                        </td>

                                        <td>
                                            #{invoice.salesOrderId}
                                        </td>

                                        <td>
                                            <span className="badge text-bg-success">
                                                Emessa
                                            </span>
                                        </td>

                                        <td className="text-end fw-semibold">
                                            {invoice.totalAmount.toLocaleString(
                                                'it-IT',
                                                {
                                                    style: 'currency',
                                                    currency: 'EUR'
                                                }
                                            )}
                                        </td>

                                    </tr>
                                ))}
                            </tbody>
                        </table>

                    </div>
                </div>
            </div>
        </>
    );
}