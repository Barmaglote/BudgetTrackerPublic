import { Component, OnDestroy, OnInit } from '@angular/core';
import { CreditItem } from '../../models/credit-item';
import { Observable, combineLatest, map } from 'rxjs';
import { Store } from '@ngrx/store';
import { ActivatedRoute } from '@angular/router';
import * as featureStore from '../../store';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import * as settingsFeatureStore from '../../../settings/store';
import { UserSettings } from 'src/app/models/user-settings';
import { AccountItem } from 'src/app/shared/models/account-item';
import { Payment } from '../../models/payment';
import { concatLatestFrom } from '@ngrx/effects';
import { TableLazyLoadEvent } from 'primeng/table';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-info',
  templateUrl: './info.component.html',
  styleUrls: ['./info.component.css']
})
export class InfoComponent implements OnInit, OnDestroy {
  public credit$: Observable<CreditItem | undefined> = this.store.select(featureStore.CreditsSelectors.credit);
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => {
      return (userSettings?.accounts ? JSON.parse(JSON.stringify(userSettings.accounts || [])) : []) as AccountItem[];
    })
  );

  public selectedAccount$ = combineLatest([this.credit$, this.accounts$]).pipe(
    map(([credit, accounts]) => {
      if (!credit || !accounts) {
        return null;
      }

      return accounts.find(x => x.id === credit.accountId);
    })
  );

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.credit$, this.userSettings$, this.accounts$
    ]);
    garbageCollector.collect();
  }

  constructor(private store: Store, private activatedRoute: ActivatedRoute) {}
  ngOnInit(): void {
    const id = this.activatedRoute.snapshot.params['id'];
    this.store.dispatch(featureStore.CreditsActions.getCredit(id));
    var initial = {} as TableLazyLoadEvent;
    initial.first = 0;
    initial.rows = 50;
    initial.sortField = 'day';
    initial.sortOrder = 1;
    this.store.dispatch(featureStore.CreditsActions.getCredits(initial));
  }

  onTransfer(transferItem: TransferItem | undefined, creditId: string | undefined) {
    if (transferItem && creditId) {
      this.store.dispatch(featureStore.CreditsActions.addTransfer(transferItem, creditId));
    }
  }

  onPlanChanged(plan: Payment[] | undefined, creditItem: CreditItem | undefined | null) {
    this.store.dispatch(featureStore.CreditsActions.storeItem({...creditItem, plan} as CreditItem));
  }

  public getAccount(accounts : AccountItem[], accountId: string | undefined) {
    if (!accounts || accounts.length === 0 || !accountId) {
      return null;
    }

    return accounts.find(x => x.id === accountId);
  }

}
