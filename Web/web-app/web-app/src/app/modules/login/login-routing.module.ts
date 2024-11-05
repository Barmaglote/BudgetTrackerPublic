import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChangePasswordRequestComponent, ChangePasswordRequestSentComponent, ConfirmationComponent, DeleteComponent, LoginComponent, RestoreComponent, RestoreConfirmedComponent, RestoreRequestComponent, RestoreSentComponent, SignupConfirmedComponent } from './containers';
import { SignupComponent } from './containers/signup/signup.component';
import { LOGIN_PROVIDERS } from './store';
import { SERVICES } from './services';
import { TransferService } from '../accounts/services';
import { authGuard } from 'src/app/core/guards/auth.guard';

export const LOGIN_ROUTE = `login`;

export const LOGIN_ROUTE_MAP = {
  root: '',
  signup: 'signup',
  signupConfirmed: 'signup/confirmed',
  confirmation: 'confirmation',
  delete: 'signup/delete',
  restoreRequest: 'restore/request',
  restoreSent: 'restore/sent',
  restoreConfirmed: 'restore/confirmed',
  restore: 'restore',
  passwordRequest: 'password/request',
  passwordRequestSent: 'password/sent',
};

const routes: Routes = [
  {
    path: LOGIN_ROUTE_MAP.root,
    component: LoginComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.signup,
    component: SignupComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.confirmation,
    component: ConfirmationComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.delete,
    canActivate: [authGuard],
    component: DeleteComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.restore,
    component: RestoreComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.restoreRequest,
    component: RestoreRequestComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.restoreSent,
    component: RestoreSentComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.restoreConfirmed,
    component: RestoreConfirmedComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.signupConfirmed,
    component: SignupConfirmedComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.passwordRequest,
    canActivate: [authGuard],
    component: ChangePasswordRequestComponent,
  },
  {
    path: LOGIN_ROUTE_MAP.passwordRequestSent,
    canActivate: [authGuard],
    component: ChangePasswordRequestSentComponent,
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...SERVICES, ...LOGIN_PROVIDERS, TransferService],
})
export class LoginRoutingModule { }
