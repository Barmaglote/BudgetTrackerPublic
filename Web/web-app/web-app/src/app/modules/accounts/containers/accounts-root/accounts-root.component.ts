import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as settingsFeatureStore from '../../../settings/store';
import * as featureStore from './../../store';
import { BehaviorSubject, Observable, Subscription, map } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import { GarbageCollector } from 'src/app/models';
import { ConfirmationService, FilterMetadata, MenuItem } from 'primeng/api';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { TableLazyLoadEvent } from 'primeng/table';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';
import { AccountItem } from 'src/app/shared/models/account-item';
import { FinancialTip } from 'src/app/shared/models/financial-tip';
import { CreditItem } from 'src/app/modules/credits/models/credit-item';
import { TranslocoService, translate } from '@ngneat/transloco';

@Component({
  selector: 'app-accounts-root',
  templateUrl: './accounts-root.component.html',
  styleUrls: ['./accounts-root.component.css']
})
export class AccountsRootComponent implements OnInit, OnDestroy {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);
  public isAddItemVisible: boolean = false;
  public staticsticsByDateIncome$: Observable<StatisticsByDate[] | undefined> = this.store.select(featureStore.AccountsSelectors.statisticsByDateIncome);
  public staticsticsByDateExpenses$: Observable<StatisticsByDate[] | undefined> = this.store.select(featureStore.AccountsSelectors.statisticsByDateExpenses);

  public transfers$: Observable<TransferItem[] | undefined> = this.store.select(featureStore.AccountsSelectors.transfers);
  public totalCount$: Observable<number | undefined> = this.store.select(featureStore.AccountsSelectors.totalCount);
  public credits$: Observable<CreditItem[] | undefined> = this.store.select(featureStore.AccountsSelectors.credits);
  public creditsItems$: BehaviorSubject<CreditItem[] | undefined> = new BehaviorSubject<CreditItem[] | undefined>([]);
  private _userSettingsSubscription: Subscription | undefined;
  private _creditsSubscription: Subscription | undefined;
  private _translocoServiceSubscription: Subscription | undefined;
  private _userSettingsTransactionsSubscription: Subscription | undefined;
  public translate = translate;

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => userSettings?.accounts || [])
  )

  constructor(private store: Store, private confirmationService: ConfirmationService, private translocoService: TranslocoService) { }
  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.userSettings$, this.staticsticsByDateIncome$, this.staticsticsByDateIncome$, this.staticsticsByDateExpenses$,
      this.transfers$, this.totalCount$, this.accounts$, this.credits$
    ]);
    garbageCollector.collect();

    if (this._userSettingsSubscription) {
      this._userSettingsSubscription.unsubscribe();
    }

    if (this._creditsSubscription) {
      this._creditsSubscription.unsubscribe();
    }

    if (this._translocoServiceSubscription) {
      this._translocoServiceSubscription.unsubscribe();
    }

    if (this._userSettingsTransactionsSubscription) {
      this._userSettingsTransactionsSubscription.unsubscribe();
    }
  }

  ngOnInit() {
    let initialStatistics = {} as TableLazyLoadEvent;
    initialStatistics.first = 0;
    initialStatistics.rows = 50;
    initialStatistics.sortField = 'day';
    initialStatistics.sortOrder = 1;

    let initialTransfers = {} as TableLazyLoadEvent;
    initialTransfers.first = 0;
    initialTransfers.rows = 10;
    initialTransfers.sortField = 'day';
    initialTransfers.sortOrder = 1;
    initialTransfers.filters = this.getTransactionFilter();

    this._userSettingsSubscription = this.userSettings$.subscribe(() => {
      this.store.dispatch(featureStore.AccountsActions.getStatisticsByDateIncome(initialStatistics));
      this.store.dispatch(featureStore.AccountsActions.getStatisticsByDateExpenses(initialStatistics));
      this.store.dispatch(featureStore.AccountsActions.getTransactions(initialTransfers));
    });

    this.store.dispatch(settingsFeatureStore.SettingsActions.getSettings());

    var initialCredits = {} as TableLazyLoadEvent;
    initialCredits.first = 0;
    initialCredits.rows = 50;
    initialCredits.sortField = 'day';
    initialCredits.sortOrder = 1;

    this.store.dispatch(featureStore.AccountsActions.getCredits(initialCredits));
    this._creditsSubscription = this.credits$.subscribe(items => this.creditsItems$.next(items));

    this._translocoServiceSubscription = this.translocoService.selectTranslateObject('financialTips.accounts').subscribe((tips: FinancialTip[]) => {
      this.financialTips = tips;
    });
  }

  onLoadData(transfers : TableLazyLoadEvent) {
    transfers.filters = this.getTransactionFilter();
    this._userSettingsTransactionsSubscription = this.userSettings$.subscribe(() => {
      this.store.dispatch(featureStore.AccountsActions.getTransactions(transfers));
    });
  }

  getTransactionFilter() {
    let filterMetadata: FilterMetadata[] = [
      {
        value: 'transfer',
        matchMode: 'eq',
        operator: 'and'
      }
    ];

    const customFilters = {
      'category': filterMetadata
    } as { [s: string]: FilterMetadata | FilterMetadata[] | undefined; };

    return customFilters;
  }

  public pageMenuItems: MenuItem[] = [
      {
        label: 'New transfer',
        icon: 'pi pi-fw pi-sync',
        command: () => { this.isAddItemVisible = true},
      },
      {
        label: 'Manage accounts',
        icon: 'pi pi-fw pi-file',
        routerLink: '/user/settings/accounts',
      }
  ];

  public financialTips: FinancialTip[] = [];

  onAdd(transfer: TransferItem) {
    this.store.dispatch(featureStore.AccountsActions.addTransfer(transfer));
    this.isAddItemVisible = false;
  }

  onSelectInfo(accountItem: AccountItem) {
    this.store.dispatch(featureStore.AccountsActions.showAccountInfo(accountItem?.id || ''));
  }

  onSelectDelete(accountItem: AccountItem) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete Account "' + accountItem.title + '"?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      key: 'accounts',
      accept: () => {
        this.store.dispatch(featureStore.AccountsActions.deleteAccount(accountItem?.id || ''));
      }
    });
  }

  onSelectEdit(accountItem: AccountItem) {
    this.store.dispatch(featureStore.AccountsActions.editAccount(accountItem?.id || ''));
  }

  onTransactionInfo(transaction: TransferItem) {
    this.store.dispatch(featureStore.AccountsActions.showTransactionInfo(transaction));
  }

  onRollback(transaction: TransferItem) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete Transaction"?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      key: 'accounts',
      accept: () => {
        this.store.dispatch(featureStore.AccountsActions.rollbackTransaction(transaction));
      }
    });
  }
}
