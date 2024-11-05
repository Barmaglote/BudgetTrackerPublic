import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { MenuItem, PrimeIcons } from 'primeng/api';

@Component({
  selector: 'app-settings-root',
  templateUrl: './settings-root.component.html',
  styleUrls: ['./settings-root.component.css']
})
export class SettingRootComponent implements OnInit {
  constructor(private store: Store) {}
  items: MenuItem[] = [];
  ngOnInit(): void {
    this.store.dispatch(featureStore.SettingsActions.getSettings());
    this.items = [
      {
          label: 'Categories',
          icon: 'pi pi-fw pi-file',
          routerLink: 'categories'
      },
      {
        label: 'Accounts',
        icon: 'pi pi-credit-card',
        routerLink: 'accounts'
      },
      {
        label: 'Language',
        icon: 'pi pi-language',
        routerLink: 'language'
      },
      {
        label: 'Templates',
        icon: 'pi pi-fw pi-pencil',
        items: [
          {
              label: 'Income',
              icon: PrimeIcons.PLUS,
              routerLink: 'templates/income'
          },
          {
            label: 'Expenses',
              icon: PrimeIcons.MINUS,
              routerLink: 'templates/expenses'
          }
        ]
      },
    ];
  }
}
