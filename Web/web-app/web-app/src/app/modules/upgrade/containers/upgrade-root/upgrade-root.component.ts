import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import * as featureSettingsStore from '../../../settings/store';
import { UserSettings } from 'src/app/models/user-settings';
import { Observable, map } from 'rxjs';
import { translate } from '@ngneat/transloco';
import { UserSubscribtion } from 'src/app/models/user-subscribtion';
import { AuthenticationService } from 'src/app/core/services';

@Component({
  selector: 'app-upgrade-root',
  templateUrl: './upgrade-root.component.html',
  styleUrls: ['./upgrade-root.component.css']
})
export class UpgradeRootComponent implements OnInit {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureSettingsStore.SettingsSelectors.userSettings);

  public isPremium$: Observable<boolean | undefined> = this.userSettings$.pipe(
    map(userSettings => {
      return (userSettings) ? userSettings.subscribtions?.some(x => x.isActive && x.planId === 'T01') : false;
    })
  );

  public premiumSubscribtion$: Observable<UserSubscribtion | undefined> = this.userSettings$.pipe(
    map(userSettings => {
      return userSettings?.subscribtions?.find(x => x.isActive && x.planId === 'T01');
    })
  );

  constructor(private store: Store, private authenticationService: AuthenticationService) { }
  ngOnInit(): void {
    this.store.dispatch(featureSettingsStore.SettingsActions.getSettings());
  }

  openPayment() {
    this.store.dispatch(featureStore.UpgradeActions.navigateToPayment({
      planId: 'T01',
      title: translate('upgrade.plans.premium.title'),
      description: translate('upgrade.plans.premium.description'),
      price: 1900,
      currency: 'usd',
    }));
  }

  unsubscribe(subscribtionId: string | undefined) {
    if (!subscribtionId) {
      console.log(this.authenticationService.getUser()?.id);
      console.log(subscribtionId);
      return;
    }
    this.store.dispatch(featureStore.UpgradeActions.unsubscribe(
      subscribtionId,
      this.authenticationService.getUser()?.id,
      this.authenticationService.getUser()?.provider
    ));
  }
}
