import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PLANNING_SERVICES } from './services';
import { ConfirmationService } from 'primeng/api';
import { PlanningRootComponent } from './containers';
import { CreditsService } from '../credits/services';
import { PLANNING_PROVIDERS } from './store';
import { DialogService } from 'primeng/dynamicdialog';
import { TransferService } from '../accounts/services';
import { ItemService } from '../item/services';

export const PLANNING_ROUTE = `planning`;

export const PLANNING_ROUTE_MAP = {
  root: '',
};

const routes: Routes = [
  {
    path: PLANNING_ROUTE_MAP.root,
    component: PlanningRootComponent,
    providers: [ConfirmationService, CreditsService, DialogService],
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...PLANNING_SERVICES, PLANNING_PROVIDERS, CreditsService, TransferService, ItemService],
})
export class PlanningRoutingModule { }
