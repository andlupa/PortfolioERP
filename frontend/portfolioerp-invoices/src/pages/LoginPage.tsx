import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { login } from '../api/authApi';
import { saveToken, saveUser } from '../auth/tokenStorage';

export default function LoginPage() {
    const navigate = useNavigate();

    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const [loading, setLoading] = useState(false);
    const [error, setError] =
        useState<string | null>(null);

    async function handleSubmit(
        event: React.FormEvent<HTMLFormElement>
    ) {
        event.preventDefault();

        setLoading(true);
        setError(null);

        try {
            const response = await login({
                username,
                password
            });

            console.log('LOGIN RESPONSE:', response);

            saveToken(response.accessToken);
            saveUser(response.user);

            navigate('/invoices');
        }
        catch (error) {
            console.error(error);

            setError(
                'Username o password non corretti.'
            );
        }
        finally {
            setLoading(false);
        }
    }

    return (
        <div
            className="row justify-content-center"
            style={{ marginTop: '80px' }}
        >
            <div className="col-md-5 col-lg-4">

                <div className="card shadow-sm">
                    <div className="card-body p-4">

                        <h1 className="h4 mb-4">
                            PortfolioERP
                        </h1>

                        <p className="text-secondary">
                            Accedi al modulo Fatturazione
                        </p>

                        {error && (
                            <div className="alert alert-danger">
                                {error}
                            </div>
                        )}

                        <form onSubmit={handleSubmit}>

                            <div className="mb-3">
                                <label
                                    htmlFor="username"
                                    className="form-label"
                                >
                                    Username
                                </label>

                                <input
                                    id="username"
                                    type="text"
                                    className="form-control"
                                    value={username}
                                    onChange={event =>
                                        setUsername(event.target.value)
                                    }
                                    required
                                />
                            </div>

                            <div className="mb-4">
                                <label
                                    htmlFor="password"
                                    className="form-label"
                                >
                                    Password
                                </label>

                                <input
                                    id="password"
                                    type="password"
                                    className="form-control"
                                    value={password}
                                    onChange={event =>
                                        setPassword(event.target.value)
                                    }
                                    required
                                />
                            </div>

                            <button
                                type="submit"
                                className="btn btn-primary w-100"
                                disabled={loading}
                            >
                                {loading
                                    ? 'Accesso...'
                                    : 'Accedi'}
                            </button>

                        </form>

                    </div>
                </div>

            </div>
        </div>
    );
}