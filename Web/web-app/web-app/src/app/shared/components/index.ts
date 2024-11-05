import { AccountTypeSelectorComponent } from "./account-type-selector/account-type-selector.component";
import { AddComponent } from "./add/add.component";
import { AdvicesComponent } from "./advices/advices.component";
import { BreadcrumbsComponent } from "./breadcrumbs/breadcrumbs.component";
import { CardComponent } from "./card/card.component";
import { CurrencySelectorComponent } from "./currency-selector/currency-selector.component";
import { NextPaymentsComponent } from "./next-payments/next-payments.component";
import { PageMenuComponent } from "./page-menu/page-menu.component";
import { StatisticsByCategoryComponent } from "./statistics-by-category/statistics-by-category.component";
import { StatisticsByDatePlainComponent } from "./statistics-by-date-plain/statistics-by-date-plain.component";
import { StatisticsByDatePlotComponent } from "./statistics-by-date-plot/statistics-by-date-plot.component";
import { StatisticsByDateComponent } from "./statistics-by-date/statistics-by-date.component";

export const COMPONENTS = [
  AddComponent,
  AdvicesComponent,
  StatisticsByDatePlainComponent,
  AccountTypeSelectorComponent,
  BreadcrumbsComponent,
  CardComponent,
  CurrencySelectorComponent,
  PageMenuComponent,
  StatisticsByCategoryComponent,
  StatisticsByDateComponent,
  StatisticsByDatePlotComponent,
  NextPaymentsComponent
]

export * from "./account-type-selector/account-type-selector.component";
export * from "./breadcrumbs/breadcrumbs.component";
export * from "./card/card.component";
export * from "./currency-selector/currency-selector.component";
export * from "./statistics-by-category/statistics-by-category.component";
export * from "./statistics-by-date/statistics-by-date.component";
export * from "./statistics-by-date-plain/statistics-by-date-plain.component";
export * from "./statistics-by-date-plot/statistics-by-date-plot.component";
export * from "./page-menu/page-menu.component";
export * from "./next-payments/next-payments.component";
