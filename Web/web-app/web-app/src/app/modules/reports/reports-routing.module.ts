import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SERVICES } from './services';
import { REPORTS_PROVIDERS } from './store';
import { ConfirmationService } from 'primeng/api';
import { ReportsRootComponent } from './containers';
import { CreditsService } from '../credits/services';

export const REPORTS_ROUTE = `user/reports`;

export const REPORTS_ROUTE_MAP = {
  root: '',
};

const routes: Routes = [
  {
    path: REPORTS_ROUTE_MAP.root,
    component: ReportsRootComponent,
    providers: [ConfirmationService],
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...SERVICES, ...REPORTS_PROVIDERS, CreditsService],
})
export class ReportsRoutingModule { }
