import { inject } from '@angular/core';
import { AuthenticationService } from './../services/authentication.service';

export const authGuard = () => {
  const authService = inject(AuthenticationService);

  if (authService.isAuthenticated()) {
    return true;
  }

  authService.toLogin(encodeURIComponent(window.location.pathname));
  return false;
};
