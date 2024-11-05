import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { ConfirmationService, MenuItem, Message, MessageService } from 'primeng/api';
import { Observable, map } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import { AccountItem } from 'src/app/shared/models/account-item';
import * as settingsFeatureStore from '../../../settings/store';
import { FinancialTip } from 'src/app/shared/models/financial-tip';
import { Category } from 'src/app/models/category';
import { AccountType } from 'src/app/shared/enums';
import { CreditItem } from '../../models/credit-item';
import * as featureStore from '../../store';
import { TableLazyLoadEvent } from 'primeng/table';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ActivateCreditComponent } from '../../dialogs';
import { PaymentInfo } from '../../models/payment-info';
import { GeneralCreditsStatistics } from '../../models/general-credits-statistics';
import { GarbageCollector } from 'src/app/models';
import { TranslocoService, translate } from '@ngneat/transloco';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.css'],
  providers: [DialogService]
})
export class RootComponent implements OnInit, OnDestroy {
  public isAddItemVisible: boolean = false;
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);
  public items$: Observable<CreditItem[] | undefined> = this.store.select(featureStore.CreditsSelectors.credits);
  public payments$: Observable<PaymentInfo[] | undefined> = this.store.select(featureStore.CreditsSelectors.payments);
  public totalCount$: Observable<number | undefined> = this.store.select(featureStore.CreditsSelectors.totalCount);
  private nonCreditAccounts: AccountItem[] | undefined;
  public generalCreditsStatistics$ : Observable<GeneralCreditsStatistics | undefined> = this.store.select(featureStore.CreditsSelectors.generalCreditsStatistics);
  public messages: Message[] = [];

  public isAllActiveOrEmpty$ = this.items$.pipe(
    map((items) => {
      if (!items || items.length == 0) {
        return false;
      }
      return items.every(x => x.isActive);
    })
  );

  private ref: DynamicDialogRef | undefined;
  public Object = Object;
  public financialTips: FinancialTip[] = [];
  constructor(
    private store: Store,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private translocoService: TranslocoService,
    public dialogService: DialogService) { }

  ngOnInit() {
    this.store.dispatch(settingsFeatureStore.SettingsActions.getSettings());
    var initial = {} as TableLazyLoadEvent;
    initial.first = 0;
    initial.rows = 50;
    initial.sortField = 'day';
    initial.sortOrder = 1;
    this.store.dispatch(featureStore.CreditsActions.getCredits(initial));

    this.accounts$.subscribe(items => {
      this.nonCreditAccounts = items ? items.filter(x => x.accountType !== AccountType.credit && x.accountType !== AccountType.creditCard) : [];
    });

    this.store.dispatch(featureStore.CreditsActions.getNextPayments());
    this.store.dispatch(featureStore.CreditsActions.getGeneralCreditsStatistics());
    this.translocoService.selectTranslateObject('financialTips.credits').subscribe((tips: FinancialTip[]) => {
      this.financialTips = tips;
    });

    this.translocoService.selectTranslateObject('credits.some_credits_are_not_active').subscribe((value) => {
      this.messages = [{ severity: 'info', summary: 'Info', detail: value }];
    });
  }

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => {
      return (userSettings?.accounts ? JSON.parse(JSON.stringify(userSettings.accounts || [])) : []) as AccountItem[];
    })
  )

  public creditAccounts$ = this.accounts$.pipe(
    map((accounts) => accounts ? accounts.filter(x => x.accountType == AccountType.credit) : [])
  )

  public categories$ = this.userSettings$.pipe(
    map((userSettings) => {
      const categories = !userSettings?.categories ? [] : userSettings.categories['credits'];
      let uniqueArray = [...new Set(categories)];

      return !uniqueArray ? [] : uniqueArray.map(x => { return { area: 'credits', category: x, color: "", hoverColor: ""} as Category})
    })
  );

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.categories$, this.userSettings$, this.accounts$,
      this.creditAccounts$, this.generalCreditsStatistics$,
      this.items$, this.payments$, this.totalCount$,
      this.ref
    ]);
    garbageCollector.collect();
  }

  public pageMenuItems: MenuItem[] = [
    {
      label: 'New credit',
      icon: 'pi pi-fw pi-sync',
      command: () => { this.isAddItemVisible = true},
    },
    {
      label: 'Manage accounts',
      icon: 'pi pi-fw pi-file',
      routerLink: '/user/settings/accounts',
    }
  ];

  onAdd(creditItem: CreditItem) {
    this.store.dispatch(featureStore.CreditsActions.storeItem(creditItem));
    this.isAddItemVisible = false;
  }

  onLoadData(event: TableLazyLoadEvent){
    this.store.dispatch(featureStore.CreditsActions.getCredits(event));
  }

  onActivate(creditItem: CreditItem) {
    this.ref = this.dialogService.open(ActivateCreditComponent, {
      header: 'Activate',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: false,
      draggable: true,
      width: '30rem',
      data: { creditItem, accounts: this.nonCreditAccounts }
    });

    this.ref.onClose.subscribe(({creditItem, accountItem}) => {
      this.store.dispatch(featureStore.CreditsActions.activateCredit(creditItem.idString, accountItem?.id));
    });
  }

  onItemUpdate(creditItem: CreditItem) {
    this.store.dispatch(featureStore.CreditsActions.storeItem(creditItem));
  }

  onDelete(creditItem: CreditItem) {
    if (creditItem.isActive === true && creditItem.plan && creditItem.plan.some(x => !x.isPaid)) {
      this.messageService.clear();
      this.messageService.add({ severity: 'error', summary: '', detail: 'Unable to delete active credit', sticky: false, life: 1000 });
      return;
    }

    this.confirmationService.confirm({
      message: 'Are you sure you want to delete Credit?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.store.dispatch(featureStore.CreditsActions.deleteCredit(creditItem));
      }
    });
  }

  onTransfer(event: any) {
    const {transferItem, creditId} = event;
    if (!event || !transferItem || !creditId) {
      return;
    }

    this.store.dispatch(featureStore.CreditsActions.addTransfer(transferItem, creditId));
  }

  onInfo(item: CreditItem) {
    this.store.dispatch(featureStore.CreditsActions.showInfo(item.idString));
  }
}
