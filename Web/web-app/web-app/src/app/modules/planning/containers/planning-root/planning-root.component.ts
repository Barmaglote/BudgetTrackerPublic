import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { BehaviorSubject, Observable, Subscription, combineLatest, map } from 'rxjs';
import { PaymentInfo } from 'src/app/modules/credits/models/payment-info';
import * as featureStore from '../../store';
import * as settingsFeatureStore from '../../../settings/store';
import { UserSettings } from 'src/app/models/user-settings';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TemplateItem } from 'src/app/shared/models/template-item';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { FinancialTip } from 'src/app/shared/models/financial-tip';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PlannedItem } from 'src/app/shared/models/planned-item';
import { AddPlannedItemComponent } from '../../dialogs';
import { Item } from 'src/app/modules/item/models';
import { interpolateColors } from 'src/app/models/color-interpolator';
import * as d3 from 'd3';
import { Category } from 'src/app/models/category';
import { interpolateInferno } from 'd3';
import { GarbageCollector } from 'src/app/models';
import { TranslocoService } from '@ngneat/transloco';

@Component({
  selector: 'app-planning-root',
  templateUrl: './planning-root.component.html',
  styleUrls: ['./planning-root.component.css'],
  providers: [DialogService]
})
export class PlanningRootComponent implements OnInit, OnDestroy {
  public payments$: Observable<PaymentInfo[] | undefined> = this.store.select(featureStore.PlanningSelectors.payments);
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);
  public plannedItems$: Observable<PlannedItem[] | undefined> = this.store.select(featureStore.PlanningSelectors.plannedItems);
  public date!: Date;
  private ref: DynamicDialogRef | undefined;
  public isAddItemVisible: boolean = false;
  public isPayItemVisible: boolean = false;
  public area$: BehaviorSubject<string | undefined> = new BehaviorSubject<string | undefined>(undefined);
  private template$: BehaviorSubject<TemplateItem | undefined> = new BehaviorSubject<TemplateItem | undefined>(undefined);
  public plannedItem$: BehaviorSubject<PlannedItem | undefined> = new BehaviorSubject<PlannedItem | undefined>(undefined);
  private _translocoServiceSubscription: Subscription | undefined;
  private _addPlannedItemSubscription: Subscription | undefined;
  private _onEditSubscription: Subscription | undefined;

  private colorRangeInfo = {
    colorStart: 0.2,
    colorEnd: 0.8,
    useEndAsStart: true,
  };

  constructor(
    private store: Store,
    public dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private translocoService: TranslocoService) { }

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => {
      return (userSettings?.accounts ? JSON.parse(JSON.stringify(userSettings.accounts || [])) : []) as AccountItem[];
    })
  )

  public templates$ = combineLatest([this.userSettings$, this.area$, this.template$]).pipe(
    map(([userSettings, area, template]) => template && area && userSettings?.templates ? JSON.parse(JSON.stringify(userSettings.templates[area].filter(x => template?.title === x.title))) as TemplateItem[] : [])
  )

  public templatesIncome$ = combineLatest([this.userSettings$]).pipe(
    map(([userSettings]) => userSettings?.templates && userSettings.templates['income'] ? JSON.parse(JSON.stringify(userSettings.templates['income'])) as TemplateItem[] : [])
  )

  public templatesExpenses$ = combineLatest([this.userSettings$]).pipe(
    map(([userSettings]) => (userSettings?.templates && userSettings?.templates['expenses']) ? JSON.parse(JSON.stringify(userSettings.templates['expenses'])) as TemplateItem[] : [])
  )

  public categories$ = combineLatest([this.userSettings$, this.area$]).pipe(
    map(([userSettings, area]) => {
      return area ? (userSettings?.categories ? JSON.parse(JSON.stringify(userSettings.categories[area] || [])) : []).map((category: any) => ({ area, category, color: "", hoverColor: ""} as Category)) : [];
    })
  );

  public categoriesColored$ = this.categories$.pipe(
    map((items) => {

      if (!items) return [];
      const colors = interpolateColors(items?.length || 0, interpolateInferno, this.colorRangeInfo);
      const lighterColors = colors.map(color => {
        const originalColor = d3.color(color);

        if (originalColor) {
            const brighter = originalColor.brighter(0.9);
            return brighter.toString();
          } else {
            return color;
          }
      });

      return items.map((x: Category, index: number) => {
        return {
          ...x,
          color: colors[index],
          hoverColor: lighterColors[index],
        }
      })
    })
  );

  ngOnInit() {
    this.store.dispatch(settingsFeatureStore.SettingsActions.getSettings());
    this.store.dispatch(featureStore.PlanningActions.getNextPayments());
    var today = new Date();
    this.store.dispatch(featureStore.PlanningActions.getPlans(today.getUTCFullYear(), today.getMonth()+1));

    this._translocoServiceSubscription = this.translocoService.selectTranslateObject('financialTips.planning').subscribe((tips: FinancialTip[]) => {
      this.financialTips = tips;
    });
  }

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.accounts$, this.categories$, this.userSettings$, this.categoriesColored$, this.payments$, this.template$,
      this.templatesExpenses$, this.area$
    ]);
    garbageCollector.collect();

    if (this._translocoServiceSubscription) {
      this._translocoServiceSubscription.unsubscribe();
    }

    if (this._addPlannedItemSubscription) {
      this._addPlannedItemSubscription.unsubscribe();
    }

    if (this._onEditSubscription) {
      this._onEditSubscription.unsubscribe();
    }
  }

  onAdd(item: Item) {
    this.isAddItemVisible = false;
    this.store.dispatch(featureStore.PlanningActions.storeItem(item, this.area$.getValue() || ''));
  }

  onAddFromPlanned(item: Item) {
    this.isPayItemVisible = false;
    this.store.dispatch(featureStore.PlanningActions.storeItemFromPlanned(item, this.area$.getValue() || '', this.plannedItem$.getValue()?.idString || ''));
  }

  public pageMenuItems: MenuItem[] = [
    {
      label: 'Add planned item',
      icon: 'pi pi-fw pi-file',
      command: () => { this.addPlannedItem()}
    },
    {
      label: 'Manage accounts',
      icon: 'pi pi-fw pi-file',
      routerLink: '/user/settings/accounts',
    },
    {
      label: 'Manage templates',
      icon: 'pi pi-fw pi-file',
      routerLink: '/user/settings/templates',
    },
    {
      label: 'Manage credits',
      icon: 'pi pi-fw pi-file',
      routerLink: '/user/credits',
    }
  ];

  public financialTips: FinancialTip[] = [];

  onTransfer(event: any) {
    const {transferItem, creditId} = event;
    if (!event || !transferItem || !creditId) {
      return;
    }

    this.store.dispatch(featureStore.PlanningActions.addTransfer(transferItem, creditId));
  }

  onMonthChange(event: any){
    const {month, year} = event;
    this.store.dispatch(featureStore.PlanningActions.getPlans(year, month));
  }

  addPlannedItem() {
    this.ref = this.dialogService.open(AddPlannedItemComponent, {
      header: 'Add planned item',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: false,
      draggable: true,
      width: '30rem'
    });

    this._addPlannedItemSubscription = this.ref.onClose.subscribe((plannedItem: PlannedItem) => {
      if (!plannedItem) {
        return;
      }
      this.store.dispatch(featureStore.PlanningActions.addPlannedItem(plannedItem));
    });
  }

  onEdit(plannedItem: PlannedItem) {
    this.ref = this.dialogService.open(AddPlannedItemComponent, {
      header: 'Edit planned item',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: false,
      draggable: true,
      width: '30rem',
      data: {plannedItem}
    });

    this._onEditSubscription = this.ref.onClose.subscribe((plannedItem: PlannedItem) => {
      if (!plannedItem) {
        return;
      }
      this.store.dispatch(featureStore.PlanningActions.addPlannedItem(plannedItem));
    });
  }

  onDelete(plannedItem: PlannedItem) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete item?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      key: 'accounts',
      accept: () => {
        this.store.dispatch(featureStore.PlanningActions.deletePlannedItem(plannedItem));
      }
    });
  }

  onPayment(event: any) {
    const {templateItem, area}= event;
    if (!area || !templateItem){
      return;
    }

    this.template$.next(templateItem);
    this.area$.next(area);
    this.isAddItemVisible = true;
  }

  onAddItem(plannedItem: PlannedItem) {
    if (!plannedItem) {
      return;
    }

    this.area$.next(plannedItem.area);
    this.plannedItem$.next(plannedItem);
    this.isPayItemVisible = true;
  }
}
