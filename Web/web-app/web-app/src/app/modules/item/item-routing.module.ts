import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RootComponent, InfoComponent } from './containers';
import { SERVICES } from './services';
import { ITEM_PROVIDERS } from './store';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TransferService } from '../accounts/services';

export const ITEM_ROUTE = `user/income`; // TODO: не универсальный путь, дает ошибку из-за income

export const ITEM_ROUTE_MAP = {
  root: '',
  info: ':id',
};

const routes: Routes = [
  {
    path: ITEM_ROUTE_MAP.root,
    component: RootComponent,
    title: "Finance management",
    providers: [MessageService, ConfirmationService, TransferService],
  },
  {
    path: ITEM_ROUTE_MAP.info,
    component: InfoComponent,
    title: "Info",
    data: { breadcrumb: 'Info'}
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...SERVICES, ...ITEM_PROVIDERS, TransferService],
})
export class ItemRoutingModule { }
