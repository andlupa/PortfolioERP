import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core'; 
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {

    const authService = inject(AuthService);

    const token = authService.getAccessToken();

    const authenticatedRequest = token
        ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` }})
        : request;

    return next(authenticatedRequest)
      .pipe(catchError((error: HttpErrorResponse) => {

          if (error.status === 401 && !request.url.includes('/auth/login')) {
            authService.logout();
          }

          return throwError(() => error);
        }
      )
    );
  };
