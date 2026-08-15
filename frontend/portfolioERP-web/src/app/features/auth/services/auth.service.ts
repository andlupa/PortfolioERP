import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { AuthUser } from '../models/auth-user';
import { LoginRequest } from '../models/login-request';
import { LoginResponse } from '../models/login-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly tokenKey = 'portfolioerp_access_token';
  private readonly userKey = 'portfolioerp_user';
  private readonly expiresKey = 'portfolioerp_expires_at';

  private readonly apiUrl =
    `${environment.apiUrl}/auth`;

  private readonly currentUserSignal =
    signal<AuthUser | null>(this.readUser());

  readonly currentUser =
    this.currentUserSignal.asReadonly();

  readonly canWrite = computed(() => {
    const user = this.currentUser();

    return user?.role === 'Admin' ||
      user?.role === 'User';
  });

  readonly isDemo = computed(
    () => this.currentUser()?.role === 'Demo'
  );

  readonly isAuthenticated = computed(() =>
    this.currentUserSignal() !== null &&
    this.hasValidToken()
  );

  login(
    request: LoginRequest
  ): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(
        `${this.apiUrl}/login`,
        request
      )
      .pipe(
        tap(response => {
          sessionStorage.setItem(
            this.tokenKey,
            response.accessToken
          );

          sessionStorage.setItem(
            this.userKey,
            JSON.stringify(response.user)
          );

          sessionStorage.setItem(
            this.expiresKey,
            response.expiresAtUtc
          );

          this.currentUserSignal.set(
            response.user
          );
        })
      );
  }

  getAccessToken(): string | null {
    if (!this.hasValidToken()) {
      return null;
    }

    return sessionStorage.getItem(
      this.tokenKey
    );
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.userKey);
    sessionStorage.removeItem(this.expiresKey);

    this.currentUserSignal.set(null);

    void this.router.navigate(['/login']);
  }

  hasValidToken(): boolean {
    const token =
      sessionStorage.getItem(this.tokenKey);

    const expiresAt =
      sessionStorage.getItem(this.expiresKey);

    if (!token || !expiresAt) {
      return false;
    }

    const expiration =
      new Date(expiresAt).getTime();

    if (
      Number.isNaN(expiration) ||
      expiration <= Date.now()
    ) {
      this.clearSession();
      return false;
    }

    return true;
  }

  private readUser(): AuthUser | null {
    if (!this.hasValidToken()) {
      return null;
    }

    const value =
      sessionStorage.getItem(this.userKey);

    if (!value) {
      return null;
    }

    try {
      return JSON.parse(value) as AuthUser;
    } catch {
      this.clearSession();
      return null;
    }
  }

  private clearSession(): void {
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.userKey);
    sessionStorage.removeItem(this.expiresKey);
  }
}
