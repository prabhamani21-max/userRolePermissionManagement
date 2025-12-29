import { Route } from '@angular/router';
import { SigninComponent } from '../signIn/signin.component';
import { noAuthGuard } from '../../../core/guards/noauth.guard ';
import { SignupComponent } from '../signup/signup.component';

export const AUTH_ROUTES: Route[] = [
  {
    path: 'sign-in',
    component: SigninComponent,
    canActivate: [noAuthGuard],
    data: { title: 'Sign In' },
  },
  {
    path: 'sign-up',
    component: SignupComponent,
    canActivate: [noAuthGuard],
    data: { title: 'Sign Up' },
  },

  {
    path: '',
    redirectTo: 'sign-in',
    pathMatch: 'full',
  },
];
