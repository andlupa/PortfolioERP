import type { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import { getUser, removeAuth } from '../auth/tokenStorage';
interface LayoutProps {
    children: ReactNode;
}

export default function Layout({ children }: LayoutProps) {

    const navigate = useNavigate();
    const user = getUser();

    function logout() {
        removeAuth();
        navigate('/login');
    }

    return (
        <>
            <nav className="navbar navbar-dark bg-dark">
                <div className="container">

                    <Link
                        className="navbar-brand fw-semibold"
                        to="/invoices"
                    >
                        PortfolioERP
                    </Link>

                    <div className="d-flex align-items-center gap-3">

                        <span className="navbar-text text-light">
                            Fatturazione (Microservizio)
                        </span>

                        {user && (
                            <span className="navbar-text text-light">
                                {user.firstName} {user.lastName}
                                {' · '}
                                <strong>{user.role}</strong>
                            </span>
                        )}

                        <a
                            className="btn btn-outline-light btn-sm"
                            href="http://localhost:4200"
                        >
                            ← ERP
                        </a>

                        <button
                            type="button"
                            className="btn btn-outline-light btn-sm"
                            onClick={logout}
                        >
                            Logout
                        </button>

                    </div>

                </div>
            </nav>

            <main className="container py-4">
                {children}
            </main>
        </>
    );
}