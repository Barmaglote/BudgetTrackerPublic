import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountsComponent, CategoriesComponent, LanguageComponent, SettingRootComponent, SettingsComponent, TemplatesComponent } from './containers';
import { SETTINGS_SERVICES } from './services';
import { ConfirmationService, MessageService } from 'primeng/api';
import { SETTINGS_PROVIDERS } from './store';

export const SETTING_ROUTE = `settings`;

export const SETTING_ROUTE_MAP = {
  root: '',
  categories: 'categories',
  templates: 'templates',
  income: 'income',
  expenses: 'expenses',
  accounts: 'accounts',
  language: 'language',
};

const routes: Routes = [
  {
    path: SETTING_ROUTE_MAP.root,
    component: SettingRootComponent,
    providers: [MessageService, ConfirmationService],
    children: [
      {
        path: SETTING_ROUTE_MAP.root,
        component: AccountsComponent,
        data: { breadcrumb: 'Settings'}
      },
      {
        path: SETTING_ROUTE_MAP.accounts,
        component: AccountsComponent,
        data: { breadcrumb: 'Accounts'}
      },
      {
        path: SETTING_ROUTE_MAP.language,
        component: LanguageComponent,
        data: { breadcrumb: 'Language'}
      },
      {
        path: SETTING_ROUTE_MAP.categories,
        component: CategoriesComponent,
        data: { breadcrumb: 'Categories'}
      },
      {
        path: SETTING_ROUTE_MAP.templates,
        component: TemplatesComponent,
        providers: [ConfirmationService],
        data: { breadcrumb: 'Templates', area: 'income'},
        children: [
          {
            path: SETTING_ROUTE_MAP.income,
            component: TemplatesComponent,
            providers: [ConfirmationService],
            data: { breadcrumb: 'Income', area: 'income'},
          },
          {
            path: SETTING_ROUTE_MAP.expenses,
            component: TemplatesComponent,
            providers: [ConfirmationService],
            data: { breadcrumb: 'Expenses', area: 'expenses'},
          }
        ]
      }
    ]
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [...SETTINGS_SERVICES, ...SETTINGS_PROVIDERS],
})
export class SettingRoutingModule { }
