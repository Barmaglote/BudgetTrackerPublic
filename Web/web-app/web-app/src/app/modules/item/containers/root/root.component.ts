import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, signal } from '@angular/core';
import { Store } from '@ngrx/store';
import { BehaviorSubject, Observable, combineLatest, map } from 'rxjs';
import * as featureStore from '../../store';
import * as settingsFeatureStore from '../../../settings/store';
import { TableLazyLoadEvent } from 'primeng/table';
import { Filter } from 'src/app/models/filter';
import { interpolateColors } from 'src/app/models/color-interpolator';
import { interpolateInferno } from 'd3';
import * as d3 from 'd3';
import { Category } from 'src/app/models/category';
import { UserSettings } from 'src/app/models/user-settings';
import { TemplateItem } from 'src/app/shared/models/template-item';
import { StatisticsByCategory } from 'src/app/models/statistics-by-category';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';
import { Item } from '../../models';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AccountItem } from 'src/app/shared/models/account-item';
import { FinancialTip } from 'src/app/shared/models/financial-tip';
import { GarbageCollector } from 'src/app/models';
import { TranslocoService } from '@ngneat/transloco';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RootComponent implements OnInit, OnDestroy {
  public staticsticsByCategory$: Observable<StatisticsByCategory[] | undefined> = this.store.select(featureStore.ItemSelectors.statisticsByCategory);
  public staticsticsByDate$: Observable<StatisticsByDate[] | undefined> = this.store.select(featureStore.ItemSelectors.statisticsByDate);
  public items$: Observable<Item[] | undefined> = this.store.select(featureStore.ItemSelectors.items);
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);
  public area$: BehaviorSubject<string | undefined> = new BehaviorSubject<string | undefined>(undefined);
  public account: AccountItem | undefined;
  public financialTips: FinancialTip[] = [];

  public selectedAccounts$ = this.staticsticsByCategory$.pipe(
    map(items => items?.map(x => x.accountId) || []),
    map(items => [... new Set(items)])
  );

  public pageMenuItems: MenuItem[] = [
    {
      label: 'Add',
      icon: 'pi pi-fw pi-file',
      command: () => this.onAddBtnClick(),
    },
    {
      label: 'Categories',
      icon: 'pi pi-cog',
      command: () => this.onManageCategoriesClick(),
    },
    {
      label: 'Templates',
      icon: 'pi pi-cog',
      routerLink: '/user/settings/templates'
    }
  ];

  /*
  public financialTips: FinancialTip[] = [
    {
      Title: "Бюджетирование",
      Tips: [
        "Ведите бюджет, чтобы знать, куда уходят ваши деньги.",
        "Определите приоритеты и распределите расходы соответственно."
      ]
    },
    {
      Title: "Экономия",
      Tips: [
        "Оцените свои расходы и ищите возможности для сокращения лишних трат.",
        "Постепенно увеличивайте сумму, которую откладываете как сбережения."
      ]
    },
    {
      Title: "Долги",
      Tips: [
        "Управляйте долгами, стремитесь к их минимизации.",
        "Оплачивайте кредиты и задолженности своевременно, чтобы избежать лишних процентов."
      ]
    },
    {
      Title: "Инвестиции",
      Tips: [
        "Изучайте возможности инвестирования для роста капитала.",
        "Разнообразьте портфель инвестиций для снижения рисков."
      ]
    },
    {
      Title: "Дополнительный доход",
      Tips: [
        "Рассмотрите возможности заработка вне основной занятости (фриланс, инвестиции, пассивный доход).",
        "Развивайте свои профессиональные навыки для повышения заработной платы."
      ]
    },
    {
      Title: "Страхование",
      Tips: [
        "Обзаведитесь страховкой на случай непредвиденных ситуаций.",
        "Периодически пересматривайте страховые полисы для обеспечения оптимального покрытия."
      ]
    },
    {
      Title: "Образование",
      Tips: [
        "Инвестируйте в свое образование для повышения квалификации.",
        "Следите за новыми трендами и рыночными возможностями."
      ]
    },
    {
      Title: "Сбережения",
      Tips: [
        "Сохраняйте аварийный фонд для неожиданных расходов.",
        "Стремитесь к накоплению достаточного объема сбережений для покрытия 3-6 месяцев расходов."
      ]
    },
    {
      Title: "Планирование на пенсию",
      Tips: [
        "Начните откладывать деньги на пенсию как можно раньше.",
        "Регулярно пересматривайте свой план пенсионного обеспечения."
      ]
    },
    {
      Title: "Самоанализ",
      Tips: [
        "Регулярно анализируйте свои финансовые цели и стратегии.",
        "Будьте готовы адаптироваться к изменяющимся обстоятельствам."
      ]
    }
  ];
  */

  public categories$ = combineLatest([this.userSettings$, this.staticsticsByCategory$, this.area$]).pipe(
    map(([userSettings, staticsticsByCategory, area]) => {
      const fromSettings = !userSettings?.categories || !area ? [] : userSettings.categories[area];
      const fromStats = !staticsticsByCategory ? [] : staticsticsByCategory.map(x => x.category);

      let combinedArray = fromSettings.concat(fromStats).filter(item => item && item.trim() !== "");
      let uniqueArray = [...new Set(combinedArray)];

      return !uniqueArray ? [] : uniqueArray.map(x => { return { area: area, category: x, color: "", hoverColor: ""} as Category})
    })
  );

  public templates$ = combineLatest([this.userSettings$, this.area$]).pipe(
    map(([userSettings, area]) => {
      return area ? (userSettings?.templates ? JSON.parse(JSON.stringify(userSettings.templates[area] || [])) : []) as TemplateItem[] : []
    })
  )

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => {
      return (userSettings?.accounts ? JSON.parse(JSON.stringify(userSettings.accounts || [])) : []) as AccountItem[];
    })
  )

  public totalCount$: Observable<number | undefined> = this.store.select(featureStore.ItemSelectors.totalCount);
  public isAddItemVisible: boolean = false;
  private lastSearch = signal<TableLazyLoadEvent | undefined>(undefined);
  private lastFilter = signal<Filter | undefined>(undefined);

  private colorRangeInfo = {
    colorStart: 0.2,
    colorEnd: 0.8,
    useEndAsStart: true,
  };

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

  public sortedItems$ = this.items$.pipe(
    map(items => items ? [...items].sort((a, b) => b.month.localeCompare(a.month)) : [])
  );

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.categories$, this.accounts$, this.area$, this.categoriesColored$, this.selectedAccounts$, this.sortedItems$,
      this.staticsticsByCategory$, this.staticsticsByDate$, this.templates$, this.totalCount$, this.userSettings$
    ]);
    garbageCollector.collect();
  }

  constructor(
    private store: Store,
    private router: Router,
    private translocoService: TranslocoService,
  ) {
  }

  ngOnInit() {
    const match = this.router.url.match(/\/user\/([^\/?]+)/);
    this.area$.next(match ? match[1] : undefined);
    var initial = {} as TableLazyLoadEvent;
    initial.first = 0;
    initial.rows = 50;
    initial.sortField = 'day';
    initial.sortOrder = 1;
    this.store.dispatch(featureStore.ItemActions.getItems(initial, this.area$.getValue() || ''));
    this.store.dispatch(featureStore.ItemActions.getStatisticsByCategory(initial, this.area$.getValue() || ''));
    this.store.dispatch(featureStore.ItemActions.getStatisticsByDate(initial, this.area$.getValue() || ''));
    this.lastSearch.set(initial);
    this.store.dispatch(settingsFeatureStore.SettingsActions.getSettings());

    this.translocoService.selectTranslateObject('financialTips.item').subscribe((tips: FinancialTip[]) => {
      this.financialTips = tips;
    }).unsubscribe();
  }

  onAddBtnClick() {
    this.isAddItemVisible = true;
  }

  onLoadData(event: TableLazyLoadEvent){

    let tableLazyLoadEvent = JSON.parse(JSON.stringify(event))
    tableLazyLoadEvent.filters = {...tableLazyLoadEvent.filters, ...this.lastFilter()};

    this.store.dispatch(featureStore.ItemActions.getItems(tableLazyLoadEvent, this.area$.getValue() || ''));
    this.store.dispatch(featureStore.ItemActions.getStatisticsByCategory(tableLazyLoadEvent, this.area$.getValue() || ''));
    this.store.dispatch(featureStore.ItemActions.getStatisticsByDate(tableLazyLoadEvent, this.area$.getValue() || ''));
    this.lastSearch.set(tableLazyLoadEvent);
  }

  onDelete(item: Item) {
    this.store.dispatch(featureStore.ItemActions.deleteItem(item, this.area$.getValue() || ''));
  }

  onUpdate(item: Item | undefined){
    if (!item) return;
    this.store.dispatch(featureStore.ItemActions.storeItem(item, this.area$.getValue() || ''));
  }

  onAdd(item: Item) {
    this.store.dispatch(featureStore.ItemActions.storeItem(item, this.area$.getValue() || ''));
    this.isAddItemVisible = false;
  }

  onFilter(filter: Filter) {
    this.lastFilter.set(filter);
    const lastSearch = this.lastSearch();
    if (lastSearch) {
      this.onLoadData(lastSearch);
    }
  }

  onInfo(item: Item | undefined){
    if (!item) { return; }
    this.store.dispatch(featureStore.ItemActions.showInfo(item, this.area$.getValue() || ''));
  }

  onInfoTransaction(item: Item | undefined){
    if (!item) { return; }
    this.store.dispatch(featureStore.ItemActions.showTransactionInfo(item));
  }

  onManageCategoriesClick(){
    this.store.dispatch(featureStore.ItemActions.openSettings('categories'));
  }

  onRollbackTransaction(item: Item) {
    this.store.dispatch(featureStore.ItemActions.deleteTransaction(item.transactionId));
  }
}
