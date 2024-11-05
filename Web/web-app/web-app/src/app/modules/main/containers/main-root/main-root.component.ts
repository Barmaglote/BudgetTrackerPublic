import { Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem, PrimeIcons } from 'primeng/api';
import { AuthenticationService } from 'src/app/core/services';
import { environment } from 'src/environments/environment';
import * as settingsFeatureStore from '../../../settings/store';
import { Store } from '@ngrx/store';
import { Observable, combineLatest, map } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import { AccountItem } from 'src/app/shared/models/account-item';
import { BriefStatistics } from 'src/app/modules/item/models/brief-statistics';
import * as featureStore from '../../store';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-main-root',
  templateUrl: './main-root.component.html',
  styleUrls: ['./main-root.component.scss']
})
export class MainRootComponent implements OnInit, OnDestroy {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);
  public briefStatistics$: Observable<BriefStatistics | undefined> = this.store.select(featureStore.MainSelectors.briefStatistics);
  items: MenuItem[] = [];
  sidebarVisible: boolean = false;
  public env = environment;

  public user$ = this.authenticationService.getUserObject();
  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => {
      return (userSettings?.accounts ? JSON.parse(JSON.stringify(userSettings.accounts || [])) : []) as AccountItem[];
    })
  )

  public incomeCurrency$ = combineLatest([this.briefStatistics$, this.accounts$]).pipe(
    map(([briefStatistics, accounts])  => {
      if (!briefStatistics || !briefStatistics?.lastIncome || !accounts) {
        return '';
      }

      return accounts.find(x => x.id == briefStatistics?.lastIncome?.accountId)?.currency;
    })
  );

  public expensesCurrency$ = combineLatest([this.briefStatistics$, this.accounts$]).pipe(
    map(([briefStatistics, accounts])  => {
      if (!briefStatistics || !briefStatistics?.lastIncome || !accounts) {
        return '';
      }

      return accounts.find(x => x.id == briefStatistics?.lastExpense?.accountId)?.currency;
    })
  );

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.accounts$, this.briefStatistics$, this.expensesCurrency$, this.incomeCurrency$, this.user$, this.userSettings$
    ]);
    garbageCollector.collect();
  }

  constructor(private authenticationService: AuthenticationService, private store: Store){
  }

  ngOnInit() {
    this.items = [
      {
        label: 'Accounts',
        icon: 'pi pi-fw pi-calendar',
        routerLink: 'accounts',
        command: () => { this.sidebarVisible = false }
      },
      {
        label: 'Income',
        icon: 'pi pi-fw pi-file',
        routerLink: 'income',
        command: () => { this.sidebarVisible = false }
      },
      {
        label: 'Expenses',
        icon: 'pi pi-fw pi-pencil',
        routerLink: 'expenses',
        command: () => { this.sidebarVisible = false }
      },
      {
        label: 'Credits',
        icon: 'pi pi-fw pi-user',
        routerLink: 'credits',
        command: () => { this.sidebarVisible = false }
      },
      {
        label: 'Planning',
        icon: 'pi pi-fw pi-calendar',
        routerLink: 'planning',
        command: () => { this.sidebarVisible = false }
      },
      {
        label: 'Reports',
        icon: 'pi pi-chart-bar',
        routerLink: 'reports',
        command: () => { this.sidebarVisible = false }
      },
      {
        label: 'Settings',
        icon: 'pi pi-fw pi-power-off',
        items: [
          {
              label: 'Catetories',
              icon: 'pi pi-fw pi-file',
              routerLink: 'settings/categories',
              command: () => { this.sidebarVisible = false },
          },
          {
            label: 'Accounts',
            icon: 'pi pi-credit-card',
            routerLink: 'settings/accounts',
            command: () => { this.sidebarVisible = false },
          },
          {
            label: 'Language',
            icon: 'pi pi-language',
            routerLink: 'settings/language',
            command: () => { this.sidebarVisible = false },
          },
          {
            label: 'Templates',
            icon: 'pi pi-fw pi-pencil',
            items: [
              {
                  label: 'Income',
                  icon: PrimeIcons.PLUS,
                  routerLink: 'settings/templates/income',
                  command: () => { this.sidebarVisible = false },
              },
              {
                  label: 'Expenses',
                  icon: PrimeIcons.MINUS,
                  routerLink: 'settings/templates/expenses',
                  command: () => { this.sidebarVisible = false },
              }
            ]
          },
        ]
      }
    ];

    this.store.dispatch(settingsFeatureStore.SettingsActions.getSettings());
    this.store.dispatch(featureStore.MainActions.getBriefStatistics());
  }
}
