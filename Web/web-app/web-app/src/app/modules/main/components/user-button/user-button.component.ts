import { Component, ElementRef, Input, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { MenuItem } from 'primeng/api';
import { Observable, map } from 'rxjs';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { UserSettings } from 'src/app/models/user-settings';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from 'src/app/modules/login/login-routing.module';
import { environment } from 'src/environments/environment';
import * as featureSettingsStore from '../../../settings/store';
import { translate } from '@ngneat/transloco';

@Component({
  selector: 'app-user-button',
  templateUrl: './user-button.component.html',
  styleUrls: ['./user-button.component.css']
})
export class UserButtonComponent implements OnInit {
  @Input() headerTemplate: TemplateRef<ElementRef> | undefined;
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureSettingsStore.SettingsSelectors.userSettings);
  items: MenuItem[];

  public isPremium$: Observable<boolean | undefined> = this.userSettings$.pipe(
    map(userSettings => {
      return (userSettings) ? userSettings.subscribtions?.some(x => x.isActive && x.planId === 'T01') : false;
    })
  );

  public menuItems$: Observable<MenuItem[]> = this.isPremium$.pipe(
    map(isPremium => {
      let provider = this.authenticationService.getUser().provider;
      const items = [
        {
          label: this.user?.name,
          icon: 'pi pi-user',
          routerLink: '/login'
        },
        {
          label: isPremium ? 'Premium' : 'Upgrade',
          icon: isPremium ? 'pi pi-star' : 'pi pi-bolt',
          routerLink: '/upgrade'
        },
        {
          label: 'Sign out',
          icon: 'pi pi-sign-out',
          command: () => { this.authenticationService.logout() }
        },
        { separator: true, visible: provider === 'budgettracker' },
        { label: 'Change password', icon: 'pi pi-user-edit', routerLink: `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.passwordRequest}`, visible: provider === 'budgettracker' },
        { label: 'Delete account', icon: 'pi pi-user-minus', routerLink: `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.delete}`, visible: provider === 'budgettracker' },
        { separator: true },
        { label: environment.version, icon: 'pi pi-cog', routerLink: '/user/settings' }
    ];

    return items;
    })
  );

  ngOnInit(): void {
    this.store.dispatch(featureSettingsStore.SettingsActions.getSettings());
  }

  public get user() {
    return this.authenticationService.getUser();
  }

  public get isAuthenticated() {
    return this.authenticationService.isAuthenticated();
  }

  constructor(private authenticationService: AuthenticationService, private router: Router, private store: Store) {

      let provider = authenticationService.getUser().provider;
      this.items = [
          {
            label: this.user?.name,
            icon: 'pi pi-user',
            routerLink: '/login'
          },
          {
            label: 'Upgrade',
            icon: 'pi pi-bolt',
            routerLink: '/upgrade'
          },
          {
            label: 'Sign out',
            icon: 'pi pi-sign-out',
            command: () => { this.authenticationService.logout() }
          },
          { separator: true, visible: provider === 'budgettracker' },
          { label: 'Change password', icon: 'pi pi-user-edit', routerLink: `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.passwordRequest}`, visible: provider === 'budgettracker' },
          { label: 'Delete account', icon: 'pi pi-user-minus', routerLink: `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.delete}`, visible: provider === 'budgettracker' },
          { separator: true },
          { label: environment.version, icon: 'pi pi-cog', routerLink: '/user/settings' }
      ];
  }

  goHome() {
    this.router.navigate(['/']);
  }
}
