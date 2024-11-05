import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { BehaviorSubject, Observable, Subscription, filter, map } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { AccountItem } from 'src/app/shared/models/account-item';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';
import { TableLazyLoadEvent } from 'primeng/table';
import * as settingsFeatureStore from '../../../settings/store';
import { UserSettings } from 'src/app/models/user-settings';
import { Item } from 'src/app/modules/item/models';
import { FilterMetadata } from 'primeng/api';
import { AccountType } from 'src/app/shared/enums';
import { GarbageCollector } from 'src/app/models';
import { Location } from '@angular/common';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsComponent implements OnInit, OnDestroy {
  public accountItem$: Observable<AccountItem | undefined> = this.store.select(featureStore.AccountsSelectors.accountItem);
  public oneMonthAgo: Date = new Date();
  public statisticsIncome$: Observable<StatisticsByDate[] | undefined> = this.store.select(featureStore.AccountsSelectors.statisticsByDateIncome);
  public statisticsExpenses$: Observable<StatisticsByDate[] | undefined> = this.store.select(featureStore.AccountsSelectors.statisticsByDateExpenses);
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);

  public income$: Observable<Item[] | undefined> = this.store.select(featureStore.AccountsSelectors.income);
  public expenses$: Observable<Item[] | undefined> = this.store.select(featureStore.AccountsSelectors.expenses);

  private _statisticsIncomeSubscription: Subscription | undefined;
  private _statisticsExpensesSubscription: Subscription | undefined;
  private _userSettingsSubscription: Subscription | undefined;

  public Math = Math;
  public AccountType = AccountType;

  constructor(private store: Store, private activatedRoute: ActivatedRoute, private location: Location) { }
  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.userSettings$, this.accountItem$, this.statisticsIncome$, this.statisticsExpenses$,
      this.income$, this.expenses$
    ]);
    garbageCollector.collect();

    if (this._statisticsIncomeSubscription) {
      this._statisticsIncomeSubscription.unsubscribe();
    }

    if (this._statisticsExpensesSubscription) {
      this._statisticsExpensesSubscription.unsubscribe();
    }

    if (this._userSettingsSubscription) {
      this._userSettingsSubscription.unsubscribe();
    }
  }

  ngOnInit(): void {
    const id = this.activatedRoute.snapshot.params['id'];
    this.store.dispatch(featureStore.AccountsActions.getAccount(id));

    this._statisticsIncomeSubscription = this.statisticsIncome$.pipe(
      filter(items => items != null && items.length > 0),
      map((items) => {
        return items?.filter(x => x.accountId === id)
      })
    ).subscribe(
      (items) => this.statisticsForIncome$.next(items)
    );

    this._statisticsExpensesSubscription = this.statisticsExpenses$.pipe(
      filter(items => items != null && items.length > 0),
      map((items) => {
        return items?.filter(x => x.accountId === id)
      })
    ).subscribe(
      (items) => this.statisticsForExpenses$.next(items)
    );

    const currentDate = new Date();
    this.oneMonthAgo.setMonth(currentDate.getMonth() - 1);

    var initial = {} as TableLazyLoadEvent;
    initial.first = 0;
    initial.rows = 50;
    initial.sortField = 'day';
    initial.sortOrder = 1;

    initial.filters = this.getAccountFilter(id);

    this.store.dispatch(featureStore.AccountsActions.getIncome(JSON.parse(JSON.stringify(initial))));
    this.store.dispatch(featureStore.AccountsActions.getExpenses(JSON.parse(JSON.stringify(initial))));

    this.userSettings$.subscribe(() => {
      this.store.dispatch(featureStore.AccountsActions.getStatisticsByDateIncome(initial));
      this.store.dispatch(featureStore.AccountsActions.getStatisticsByDateExpenses(initial));
    })

    this.store.dispatch(settingsFeatureStore.SettingsActions.getSettings());
  }

  public statisticsForIncome$: BehaviorSubject<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  public statisticsForExpenses$: BehaviorSubject<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  public difference$: Observable<number> = this.accountItem$.pipe(map(x => (x?.quantity || 0) - (x?.initial || 0)));

  getAccountFilter(accountId: string) {
    let filterMetadata: FilterMetadata[] = [
      {
        value: accountId,
        matchMode: 'eq',
        operator: 'and'
      }
    ];

    const customFilters = {
      'accountId': filterMetadata
    } as { [s: string]: FilterMetadata | FilterMetadata[] | undefined; };

    return customFilters;
  }

  backClicked() {
    this.location.back();
  }
}
