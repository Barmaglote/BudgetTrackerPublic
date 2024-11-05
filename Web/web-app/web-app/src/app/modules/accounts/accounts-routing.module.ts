import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountsRootComponent, DetailsComponent } from './containers';
import { SERVICES } from './services';
import { ACCOUNTS_PROVIDERS } from './store';
import { SettingsService } from '../settings/services';
import { ConfirmationService } from 'primeng/api';
import { TransactionInfoComponent } from './containers/transaction-info/transaction-info.component';

export const ACCOUNTS_ROUTE = `user/accounts`;

export const ACCOUNTS_ROUTE_MAP = {
  root: '',
  accounts: 'summary',
  details: ':id',
  info: 'info/:id',
};

const routes: Routes = [
  {
    path: ACCOUNTS_ROUTE_MAP.root,
    component: AccountsRootComponent,
    providers: [ConfirmationService],
    children: [
      {
        path: ACCOUNTS_ROUTE_MAP.root,
        component: AccountsRootComponent,
        providers: [ConfirmationService],
        data: { breadcrumb: 'Summary'}
      }
    ]
  },
  {
    path: ACCOUNTS_ROUTE_MAP.details,
    component: DetailsComponent,
    data: { breadcrumb: 'Details'}
  },
  {
    path: ACCOUNTS_ROUTE_MAP.info,
    component: TransactionInfoComponent,
    data: { breadcrumb: 'Info'}
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...SERVICES, ...ACCOUNTS_PROVIDERS, SettingsService],
})
export class AccountsRoutingModule { }
