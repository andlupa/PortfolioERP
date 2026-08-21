export interface AuthUser {
    id: number;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
}

const TOKEN_KEY = 'portfolioerp_token';
const USER_KEY = 'portfolioerp_user';

export function saveToken(token: string): void {
    if (!token) {
        throw new Error('Token non valido.');
    }

    localStorage.setItem(TOKEN_KEY, token);
}

export function getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
}

export function saveUser(user: AuthUser): void {
    localStorage.setItem(
        USER_KEY,
        JSON.stringify(user)
    );
}

export function getUser(): AuthUser | null {
    const value = localStorage.getItem(USER_KEY);

    if (!value) {
        return null;
    }

    try {
        return JSON.parse(value) as AuthUser;
    } catch {
        return null;
    }
}

export function removeAuth(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
}