import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, combineLatest, map} from 'rxjs';
import * as settingsFeatureStore from '../../../settings/store';
import { Store } from '@ngrx/store';
import { UserSettings } from 'src/app/models/user-settings';
import * as featureStore from '../../store';
import { RowInfo } from '../../models/row-info';
import { RegularStatistics } from 'src/app/modules/item/models/regualar-payments';
import { RowInfoAggrigated } from '../../models/row-info-aggrigated';
import { Item } from 'src/app/modules/item/models';
import { TableLazyLoadEvent } from 'primeng/table';
import { CreditItem } from 'src/app/modules/credits/models/credit-item';
import { AccountItem } from 'src/app/shared/models/account-item';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-reports-root',
  templateUrl: './reports-root.component.html',
  styleUrls: ['./reports-root.component.css']
})
export class ReportsRootComponent implements OnInit, OnDestroy {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);
  public regularPayments$: Observable<RegularStatistics | undefined> = this.store.select(featureStore.ReportsSelectors.regularPayments);
  public credits$: Observable<CreditItem[] | undefined> = this.store.select(featureStore.ReportsSelectors.credits);
  public date: Date[] | undefined;
  public currentYear: number = (new Date()).getFullYear();

  private ExpensesRegular$ = this.userSettings$.pipe(
    map(userSettings => this.convertTemplates(userSettings, 'expenses')),
    map((items) => this.groupItems(items)),
    map(groupedItems => groupedItems ? Object.values(groupedItems): null)
  );

  private IncomesRegular$ = this.userSettings$.pipe(
    map(userSettings => this.convertTemplates(userSettings, 'income')),
    map((items) => this.groupItems(items)),
    map(groupedItems => groupedItems ? Object.values(groupedItems): null)
  );

  private Expenses$ = combineLatest([this.regularPayments$, this.userSettings$]).pipe(
    map(([items, userSettings]) => this.convertItems(items?.expenses, userSettings)),
    map(groupedItems => groupedItems ? Object.values(groupedItems): null),
  );

  private Incomes$ = combineLatest([this.regularPayments$, this.userSettings$]).pipe(
    map(([items, userSettings]) => this.convertItems(items?.incomes, userSettings)),
    map(groupedItems => groupedItems ? Object.values(groupedItems): null),
  );

  public AggrigatedExpenses$ = combineLatest([this.ExpensesRegular$, this.Expenses$]).pipe(
    map(([plan, actual]) => this.mergeItems(plan, actual))
  );

  public AggrigatedIncomes$ = combineLatest([this.IncomesRegular$, this.Incomes$]).pipe(
    map(([plan, actual]) => this.mergeItems(plan, actual))
  );

  public ActiveCredits$ = this.credits$.pipe(
    map(items => items ? items.filter(x => x.isActive) : []),
  )

  public Accounts$ = this.userSettings$.pipe(
    map((userSettings) => {
      return (userSettings?.accounts ? JSON.parse(JSON.stringify(userSettings.accounts || [])) : []) as AccountItem[];
    })
  )

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.Accounts$, this.ActiveCredits$, this.AggrigatedExpenses$, this.AggrigatedIncomes$, this.AggrigatedIncomes$, this.Expenses$,
      this.ExpensesRegular$, this.Incomes$, this.IncomesRegular$, this.credits$, this.userSettings$
    ]);
    garbageCollector.collect();
  }

  private mergeItems(plan: RowInfo[] | null, actual: RowInfo[] | null) {
    const uniqueCategoriesAndCurrencies = Array.from(new Set([...(plan || []).map(item => `${item.category}_${item.currency}`), ...(actual || []).map(item => `${item.category}_${item.currency}`)]));

    return uniqueCategoriesAndCurrencies.map(key => {
      const [category, currency] = key.split('_');
      const planItem = plan?.find(item => item.category === category && item.currency === currency);
      const actualItem = actual?.find(item => item.category === category && item.currency === currency);

      return {
        plan: { ...(planItem || {}) },
        actual: { ...(actualItem || {}) },
        category,
        currency,
      } as RowInfoAggrigated;
    });
  }

  private groupItems(items: RowInfo[] | null) {

    if (!items) { return null; }
    var groupedValues: Record<string, RowInfo> = {};

    items.forEach(item => {
      if (!groupedValues[item.category+'_:_'+item.currency]) {
        groupedValues[item.category+'_:_'+item.currency] = item;
      } else {
        var current = JSON.parse(JSON.stringify(groupedValues[item.category+'_:_'+item.currency]));
        groupedValues[item.category+'_:_'+item.currency] = {
          jan: current.jan + item.jan,
          feb: current.jan + item.feb,
          mar: current.jan + item.mar,
          apr: current.jan + item.apr,
          may: current.jan + item.may,
          jun: current.jan + item.jun,
          jul: current.jan + item.jul,
          aug: current.jan + item.aug,
          sep: current.jan + item.sep,
          oct: current.jan + item.oct,
          nov: current.jan + item.nov,
          dec: current.jan + item.dec,
          currency: item?.currency,
          category: item?.category,
          year: current.year + (item.jan+item.feb+item.mar+item.apr+item.may+item.jun+item.jul+item.aug+item.sep+item.nov+item.oct+item.dec)
        }
      }
    })

    return groupedValues;
  };

  private convertItems(items: Item[] | undefined | null, userSettings: UserSettings | undefined) {
    if (!items) {
      return null;
    }

    if (!userSettings?.accounts) {
      return null;
    }

    var groupedValues: Record<string, RowInfo> = {};
    items.forEach(x => {
      var account = userSettings?.accounts?.find(a => a.id == x.accountId);

      const month = new Date(x.date).getMonth()+1;

      if (!groupedValues[x.category+'_:_'+account?.currency]) {
        groupedValues[x.category+'_:_'+account?.currency] = {
            jan: (month === 1) ? x.quantity : 0,
            feb: (month === 2) ? x.quantity : 0,
            mar: (month === 3) ? x.quantity : 0,
            apr: (month === 4) ? x.quantity : 0,
            may: (month === 5) ? x.quantity : 0,
            jun: (month === 6) ? x.quantity : 0,
            jul: (month === 7) ? x.quantity : 0,
            aug: (month === 8) ? x.quantity : 0,
            sep: (month === 9) ? x.quantity : 0,
            oct: (month === 10) ? x.quantity : 0,
            nov: (month === 11) ? x.quantity : 0,
            dec: (month === 12) ? x.quantity : 0,
            currency: account?.currency || '',
            category: x?.category,
            year: x.quantity
          };
      } else {
        var current = JSON.parse(JSON.stringify(groupedValues[x.category+'_:_'+account?.currency]));
        groupedValues[x.category+'_:_'+account?.currency] = {
          jan: current.jan + ((month === 1) ? x.quantity : 0),
          feb: current.feb + ((month === 2) ? x.quantity : 0),
          mar: current.mar + ((month === 3) ? x.quantity : 0),
          apr: current.apr + ((month === 4) ? x.quantity : 0),
          may: current.may + ((month === 5) ? x.quantity : 0),
          jun: current.jun + ((month === 6) ? x.quantity : 0),
          jul: current.jul + ((month === 7) ? x.quantity : 0),
          aug: current.aug + ((month === 8) ? x.quantity : 0),
          sep: current.sep + ((month === 9) ? x.quantity : 0),
          oct: current.oct + ((month === 10) ? x.quantity : 0),
          nov: current.nov + ((month === 11) ? x.quantity : 0),
          dec: current.dec + ((month === 12) ? x.quantity : 0),
          currency: account?.currency || '',
          category: x?.category,
          year: current.year + x.quantity
        };
      }
    });
    return groupedValues;
  };

  private convertTemplates(userSettings: UserSettings | undefined, area: string) {
    if (!userSettings?.templates || !userSettings?.accounts) { return []}

    var rex = userSettings?.templates[area]?.filter(x => x.isRegular == true && x.category);
    if (!rex) { return []}

    return rex.map(x => {
      var account = userSettings?.accounts?.find(a => a.id == x.accountId);

      return {
        jan: x.quantity,
        feb: x.quantity,
        mar: x.quantity,
        apr: x.quantity,
        may: x.quantity,
        jun: x.quantity,
        jul: x.quantity,
        aug: x.quantity,
        sep: x.quantity,
        oct: x.quantity,
        nov: x.quantity,
        dec: x.quantity,
        category: x.category,
        currency: account?.currency,
        year: x.quantity
      } as RowInfo;
    });
  };

  constructor(private store: Store) { }

  ngOnInit() {
    var initial = {} as TableLazyLoadEvent;
    initial.first = 0;
    initial.rows = 50;
    initial.sortField = 'day';
    initial.sortOrder = 1;
    this.store.dispatch(featureStore.ReportsActions.getCredits(initial));

    var today = new Date();
    this.store.dispatch(featureStore.ReportsActions.getRegularPayments(today.getFullYear()));
  }

  onYearChange(date: Date) {
    this.currentYear = (new Date(date)).getFullYear();
    this.store.dispatch(featureStore.ReportsActions.getRegularPayments(this.currentYear));
  }
}
