import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';

import {
  AuthService
} from '../../features/auth/services/auth.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const user = authService.currentUser();

  if (
    authService.hasValidToken() &&
    user?.role === 'Admin'
  ) {
    return true;
  }

  return router.createUrlTree([
    '/dashboard'
  ]);
};
