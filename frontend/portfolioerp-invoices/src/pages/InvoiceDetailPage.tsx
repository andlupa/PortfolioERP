import { useEffect, useState } from 'react';
import {
    Link,
    useParams
} from 'react-router-dom';

import { getInvoiceById } from '../api/invoiceApi';
import type { Invoice } from '../models/invoice';

export default function InvoiceDetailPage() {
    const { id } = useParams();

    const [invoice, setInvoice] =
        useState<Invoice | null>(null);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        const invoiceId = Number(id);

        if (!Number.isInteger(invoiceId) || invoiceId <= 0) {
            setError('Identificativo fattura non valido.');
            setLoading(false);
            return;
        }

        getInvoiceById(invoiceId)
            .then(data => {
                setInvoice(data);
            })
            .catch(error => {
                console.error(error);
                setError('Impossibile caricare la fattura.');
            })
            .finally(() => {
                setLoading(false);
            });
    }, [id]);

    if (loading) {
        return <p>Caricamento fattura...</p>;
    }

    if (error) {
        return (
            <div>
                <p>{error}</p>
                <Link to="/invoices">
                    Torna alle fatture
                </Link>
            </div>
        );
    }

    if (!invoice) {
        return null;
    }

    return (
        <>
            <div className="mb-3">
                <Link
                    to="/invoices"
                    className="text-decoration-none"
                >
                    ← Torna alle fatture
                </Link>
            </div>

            <div className="d-flex justify-content-between align-items-start mb-4">
                <div>
                    <h1 className="h3 mb-1">
                        {invoice.invoiceNumber}
                    </h1>

                    <span className="text-secondary">
                        Ordine #{invoice.salesOrderId}
                    </span>
                </div>

                <span className="badge text-bg-success fs-6">
                    Emessa
                </span>
            </div>

            <div className="card shadow-sm mb-4">
                <div className="card-body">
                    <div className="row">

                        <div className="col-md-6">
                            <div className="text-secondary small">
                                Cliente
                            </div>

                            <div className="fw-semibold">
                                {invoice.customerName}
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="text-secondary small">
                                Data fattura
                            </div>

                            <div>
                                {new Date(
                                    invoice.invoiceDateUtc
                                ).toLocaleDateString('it-IT')}
                            </div>
                        </div>

                        <div className="col-md-3">
                            <div className="text-secondary small">
                                Ordine
                            </div>

                            <div>
                                #{invoice.salesOrderId}
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <h2 className="h5 mb-3">
                Righe fattura
            </h2>

            <div className="card shadow-sm mb-4">
                <div className="table-responsive">

                    <table className="table table-hover align-middle mb-0">

                        <thead className="table-light">
                            <tr>
                                <th>Codice</th>
                                <th>Descrizione</th>

                                <th className="text-end">
                                    Quantità
                                </th>

                                <th className="text-end">
                                    Prezzo
                                </th>

                                <th className="text-end">
                                    Sconto
                                </th>

                                <th className="text-end">
                                    IVA
                                </th>

                                <th className="text-end">
                                    Totale
                                </th>
                            </tr>
                        </thead>

                        <tbody>
                            {invoice.lines.map(line => (
                                <tr key={line.id}>

                                    <td className="fw-semibold">
                                        {line.productCode}
                                    </td>

                                    <td>
                                        {line.description}
                                    </td>

                                    <td className="text-end">
                                        {line.quantity}
                                    </td>

                                    <td className="text-end">
                                        {line.unitPrice.toLocaleString(
                                            'it-IT',
                                            {
                                                style: 'currency',
                                                currency: 'EUR'
                                            }
                                        )}
                                    </td>

                                    <td className="text-end">
                                        {line.discountPercentage}%
                                    </td>

                                    <td className="text-end">
                                        {line.vatPercentage}%
                                    </td>

                                    <td className="text-end fw-semibold">
                                        {line.totalAmount.toLocaleString(
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

            <div className="row justify-content-end">
                <div className="col-md-5 col-lg-4">

                    <div className="card shadow-sm">
                        <div className="card-body">

                            <div className="d-flex justify-content-between mb-2">
                                <span>Subtotale</span>

                                <span>
                                    {invoice.subtotal.toLocaleString(
                                        'it-IT',
                                        {
                                            style: 'currency',
                                            currency: 'EUR'
                                        }
                                    )}
                                </span>
                            </div>

                            <div className="d-flex justify-content-between mb-2">
                                <span>Sconti</span>

                                <span>
                                    {invoice.discountAmount.toLocaleString(
                                        'it-IT',
                                        {
                                            style: 'currency',
                                            currency: 'EUR'
                                        }
                                    )}
                                </span>
                            </div>

                            <div className="d-flex justify-content-between mb-3">
                                <span>IVA</span>

                                <span>
                                    {invoice.taxAmount.toLocaleString(
                                        'it-IT',
                                        {
                                            style: 'currency',
                                            currency: 'EUR'
                                        }
                                    )}
                                </span>
                            </div>

                            <hr />

                            <div className="d-flex justify-content-between fs-5 fw-bold">
                                <span>Totale</span>

                                <span>
                                    {invoice.totalAmount.toLocaleString(
                                        'it-IT',
                                        {
                                            style: 'currency',
                                            currency: 'EUR'
                                        }
                                    )}
                                </span>
                            </div>

                        </div>
                    </div>

                </div>
            </div>
        </>
    );
}