import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthenticationService } from '../services/auth.service';
import { Router } from '@angular/router';

export const noAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthenticationService);
  const router = inject(Router);

  console.log('noAuthGuard: isAuthenticated:', authService.isAuthenticated());
  if (!authService.isAuthenticated()) {
    console.log('noAuthGuard: not authenticated, allowing access');
    return true;
  }
  // Redirect to home or dashboard if already authenticated
  console.log('noAuthGuard: authenticated, redirecting to userRolePermission/admin');
  return router.parseUrl('userRolePermission/admin');
};
