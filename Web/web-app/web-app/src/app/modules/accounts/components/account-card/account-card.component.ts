import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { BehaviorSubject, Observable, Subscription, combineLatest, map } from 'rxjs';
import { filter } from 'rxjs/operators';
import { GarbageCollector } from 'src/app/models';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';
import { CreditItem } from 'src/app/modules/credits/models/credit-item';
import { AccountType } from 'src/app/shared/enums';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  selector: 'app-account-card',
  templateUrl: './account-card.component.html',
  styleUrls: ['./account-card.component.css']
})
export class AccountCardComponent implements OnInit, OnDestroy {
  public oneMonthAgo: Date = new Date();
  @Input({required:true}) statisticsIncome$: Observable<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  @Input({required:true}) statisticsExpenses$: Observable<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  @Input({required:true}) item!: AccountItem;
  @Input() credits$: Observable<CreditItem[] | undefined> = new BehaviorSubject<CreditItem[] | undefined>([]);
  @Output() selectInfo = new EventEmitter<AccountItem>();
  @Output() selectDelete = new EventEmitter<AccountItem>();
  @Output() selectEdit = new EventEmitter<AccountItem>();
  public AccountType = AccountType;
  public debtPlanned: number = 0;
  public debtPaid: number = 0;
  private _statisticsCategoriesSubscription: Subscription | undefined;
  private _statisticsIncomeSubscription: Subscription | undefined;
  private _statisticsExpensesSubscription: Subscription | undefined;
  private _statisticsCreditsSubscription: Subscription | undefined;

  ngOnInit(): void {
    this._statisticsIncomeSubscription = this.statisticsIncome$.pipe(
      filter(items => items != null && items.length > 0),
      map((items) => {
        return items?.filter(x => x.accountId === this.item.id)
      })
    ).subscribe(
      (items) => this.statisticsForIncome$.next(items)
    );

    this._statisticsExpensesSubscription = this.statisticsExpenses$.pipe(
      filter(items => items != null && items.length > 0),
      map((items) => {
        return items?.filter(x => x.accountId === this.item.id)
      })
    ).subscribe(
      (items) => this.statisticsForExpenses$.next(items)
    );

    const currentDate = new Date();
    this.oneMonthAgo.setMonth(currentDate.getMonth() - 1);

    this._statisticsCategoriesSubscription = combineLatest([this.statisticsIncome$,this.statisticsExpenses$])
      .pipe(
        map(([statisticsIncome, statisticsExpenses]) => {
          let statisticsIncomeLastMonth = statisticsIncome?.filter(x => new Date(x.date) > this.oneMonthAgo && x.accountId === this.item.id) || [];
          let statisticsExpensesLastMonth = statisticsExpenses?.filter(x => new Date(x.date) > this.oneMonthAgo && x.accountId === this.item.id) || [];
          return {statisticsIncomeLastMonth, statisticsExpensesLastMonth};
        }),
        map(({statisticsIncomeLastMonth, statisticsExpensesLastMonth}) =>
          statisticsIncomeLastMonth.reduce((acc, v) => acc + v.quantity, 0) - statisticsExpensesLastMonth.reduce((acc, v) => acc + v.quantity, 0)
        )
      ).subscribe(
        (diff) => this.difference$.next(diff)
      )

    this._statisticsCreditsSubscription = this.credits$.subscribe(credits => {
      const filtered = credits ? credits.filter(x => x.accountId == this.item.id && x.isActive) : [];
      const plans = filtered.flatMap(c => c.plan);
      this.debtPlanned = plans.reduce((accumulator, currentValue) => accumulator + (currentValue ? currentValue.quantity : 0), 0);
      const plansPaid = plans.filter(c => c?.isPaid);
      this.debtPaid = plansPaid.reduce((accumulator, currentValue) => accumulator + (currentValue ? currentValue.quantity : 0), 0)
    });
  }

  public statisticsForIncome$: BehaviorSubject<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  public statisticsForExpenses$: BehaviorSubject<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  public difference$: BehaviorSubject<number> = new BehaviorSubject<number>(0);

  private currentItem!: AccountItem;
  public Math = Math;

  selectItem(item: AccountItem) {
    this.currentItem = item;
  }

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.statisticsIncome$, this.statisticsExpenses$, this.statisticsForIncome$,
      this.statisticsForExpenses$, this.difference$
    ]);
    garbageCollector.collect();

    if (this._statisticsIncomeSubscription) {
      this._statisticsIncomeSubscription.unsubscribe();
    }

    if (this._statisticsExpensesSubscription) {
      this._statisticsExpensesSubscription.unsubscribe();
    }

    if (this._statisticsCategoriesSubscription) {
      this._statisticsCategoriesSubscription.unsubscribe();
    }

    if (this._statisticsCreditsSubscription) {
      this._statisticsCreditsSubscription.unsubscribe();
    }
  }

  public menuItems: MenuItem[] = [
    {
      label: 'Info',
      icon: 'pi pi-info',
      command: () => {
        this.selectInfo.emit(this.currentItem);
      }
    },
    {
      label: 'Edit',
      icon: 'pi pi-file-edit',
      command: () => {
        this.selectEdit.emit(this.currentItem);
      }
    },
    {
      label: 'Delete',
      icon: 'pi pi-trash',
      command: () => {
        this.selectDelete.emit(this.currentItem);
      }
    },
  ]

  getAccountTypeText(accountType?: AccountType): string {
    switch (accountType) {
      case AccountType.cash:
        return 'Cash';
      case AccountType.debitCard:
        return 'Debit Card';
      case AccountType.creditCard:
        return 'Credit Card';
      case AccountType.deposit:
        return 'Deposit';
      case AccountType.credit:
        return 'Credit';
      default:
        return 'Unknown';
    }
  }
}
