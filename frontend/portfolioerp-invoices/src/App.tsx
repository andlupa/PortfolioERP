import { Route, Routes } from 'react-router-dom';

import Layout from './Components/Layout';

import ProtectedRoute from './auth/ProtectedRoute';

import LoginPage from './pages/LoginPage';
import InvoiceListPage from './pages/InvoiceListPage';
import InvoiceDetailPage from './pages/InvoiceDetailPage';

function App() {
    return (
        <Layout>
            <Routes>

                <Route
                    path="/login"
                    element={<LoginPage />}
                />

                <Route
                    path="/"
                    element={
                        <ProtectedRoute>
                            <InvoiceListPage />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/invoices"
                    element={
                        <ProtectedRoute>
                            <InvoiceListPage />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/invoices/:id"
                    element={
                        <ProtectedRoute>
                            <InvoiceDetailPage />
                        </ProtectedRoute>
                    }
                />

            </Routes>
        </Layout>
    );
}

export default App;