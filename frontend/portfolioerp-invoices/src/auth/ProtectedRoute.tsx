import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';

import { getToken } from './tokenStorage';

interface ProtectedRouteProps {
    children: ReactNode;
}

export default function ProtectedRoute({
    children
}: ProtectedRouteProps) {

    const token = getToken();

    if (!token) {
        return (
            <Navigate
                to="/login"
                replace
            />
        );
    }

    return children;
}