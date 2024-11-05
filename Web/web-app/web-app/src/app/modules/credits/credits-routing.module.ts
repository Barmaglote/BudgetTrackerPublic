import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SERVICES } from './services';
import { ConfirmationService, MessageService } from 'primeng/api';
import { StatisticsService, TransferService } from '../accounts/services';
import { InfoComponent, RootComponent } from './containers';

export const CREDITS_ROUTE = `user/credits`;

export const CREDITS_ROUTE_MAP = {
  root: '',
  info: ':id',
};

const routes: Routes = [
  {
    path: CREDITS_ROUTE_MAP.root,
    component: RootComponent,
    title: "Credit management",
    providers: [MessageService, ConfirmationService, TransferService],
  },
  {
    path: CREDITS_ROUTE_MAP.info,
    component: InfoComponent,
    title: "Info",
    data: { breadcrumb: 'Info'},
    providers: [MessageService, ConfirmationService, TransferService],
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...SERVICES, TransferService, StatisticsService],
})
export class CreditsRoutingModule { }
