import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainRootComponent, UserMenuComponent } from './containers';
import { MAIN_PROVIDERS } from './store';
import { StatisticsService } from '../item/services';

const routes: Routes = [
  {
    path: '',
    component: MainRootComponent,
    children: [
      {
        path: '',
        component: UserMenuComponent,
        data: { headerTitle: 'User', breadcrumb: 'User' },
      },
      {
        path: 'income',
        loadChildren: () => import('../item/item.module').then((m) => m.ItemModule),
        data: { headerTitle: 'Income', breadcrumb: 'Income' },
      },
      {
        path: 'expenses',
        loadChildren: () => import('../item/item.module').then((m) => m.ItemModule),
        data: { headerTitle: 'Expenses', breadcrumb: 'Expenses' },
      },
      {
        path: 'credits',
        loadChildren: () => import('../credits/credits.module').then((m) => m.CreditsModule),
        data: { headerTitle: 'Credits', breadcrumb: 'Credits' },
      },
      {
        path: 'accounts',
        loadChildren: () => import('../accounts/accounts.module').then((m) => m.AccountsModule),
        data: { headerTitle: 'Accounts', breadcrumb: 'Accounts' },
      },
      {
        path: 'planning',
        loadChildren: () => import('../planning/planning.module').then((m) => m.PlanningModule),
        data: { headerTitle: 'Planning', breadcrumb: 'Planning' },
      },
      {
        path: 'reports',
        loadChildren: () => import('../reports/reports.module').then((m) => m.ReportsModule),
        data: { headerTitle: 'Reports', breadcrumb: 'Reports' },
      },
      {
        path: 'settings',
        loadChildren: () => import('../settings/settings.module').then((m) => m.SettingsModule),
        data: { headerTitle: 'Settings', breadcrumb: 'Settings' },
      },
    ]
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...MAIN_PROVIDERS, StatisticsService],
})
export class MainRoutingModule { }

