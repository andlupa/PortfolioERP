const ERP_API_URL = 'http://localhost:5063';

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    accessToken: string;
    expiresAtUtc: string;

    user: {
        id: number;
        username: string;
        email: string;
        firstName: string;
        lastName: string;
        role: string;
    };
}

export async function login(
    request: LoginRequest
): Promise<LoginResponse> {

    const response = await fetch(
        `${ERP_API_URL}/api/auth/login`,
        {
            method: 'POST',

            headers: {
                'Content-Type': 'application/json'
            },

            body: JSON.stringify(request)
        }
    );

    if (!response.ok) {
        throw new Error('Credenziali non valide.');
    }

    return response.json();
}