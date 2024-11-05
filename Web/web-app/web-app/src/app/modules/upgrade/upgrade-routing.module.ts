import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UPGRADE_SERVICES } from './services';
import { PaymentComponent, PaymentFailedComponent, PaymentSuccessComponent, UnsubscribeComponent, UpgradeRootComponent } from './containers';
import { UPGRADE_PROVIDERS } from './store';

export const UPGRADE_ROUTE = `upgrade`;

export const UPGRADE_ROUTE_MAP = {
  root: '',
  payment: 'payment',
  paymentSuccess: 'success',
  paymentFailed: 'failed',
  unsubscribe: 'unsubscribe'
};

const routes: Routes = [
  {
    path: UPGRADE_ROUTE_MAP.root,
    component: UpgradeRootComponent,
    data: { breadcrumb: 'Upgrade'},
    providers: [...UPGRADE_PROVIDERS],
  },
  {
    path: UPGRADE_ROUTE_MAP.payment,
    component: PaymentComponent,
    data: { breadcrumb: 'Payment'},
  },
  {
    path: UPGRADE_ROUTE_MAP.paymentSuccess,
    component: PaymentSuccessComponent,
    data: { breadcrumb: 'Success'},
  },
  {
    path: UPGRADE_ROUTE_MAP.paymentFailed,
    component: PaymentFailedComponent,
    data: { breadcrumb: 'Failed'},
  },
  {
    path: UPGRADE_ROUTE_MAP.unsubscribe,
    component: UnsubscribeComponent,
    data: { breadcrumb: 'Unsubscribe'},
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...UPGRADE_SERVICES, ...UPGRADE_PROVIDERS],
})
export class UpgradeRoutingModule { }
