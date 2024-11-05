import { createActionGroup, emptyProps } from '@ngrx/store';
import { TableLazyLoadEvent } from 'primeng/table';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { Item } from '../../item/models';
import { CreditItem } from '../../credits/models/credit-item';

export const AccountsActions = createActionGroup({
  source: 'Accounts',
  events: {
    'Clean Store': emptyProps(),
    'Add Transfer': (transfer: TransferItem) => ({transfer}),
    'Add Transfer Success':  (result: boolean) => ({result}),
    'Show Transaction Info': (transfer: TransferItem) => ({transfer}),
    'Failure': (text: string, error: any) => ({text, error}),
    'Get Statistics By Date Income': (tableLazyLoadEvent: TableLazyLoadEvent) => ({tableLazyLoadEvent}),
    'Get Statistics By Date Income Success': (statisticsByDate: StatisticsByDate[]) => ({statisticsByDate}),
    'Get Statistics By Date Expenses': (tableLazyLoadEvent: TableLazyLoadEvent) => ({tableLazyLoadEvent}),
    'Get Statistics By Date Expenses Success': (statisticsByDate: StatisticsByDate[]) => ({statisticsByDate}),
    'Get Account': (id: string) => ({id}),
    'Get Account Success': (accountItem: AccountItem) => ({accountItem}),
    'Show Account Info': (id: string) => ({id}),
    'Delete Account': (id: string) => ({id}),
    'Get Transactions': (tableLazyLoadEvent: TableLazyLoadEvent) => ({tableLazyLoadEvent}),
    'Get Transactions Success': (transfers: TransferItem[], totalCount: number) => ({transfers, totalCount}),
    'Get Transaction': (id: string) => ({id}),
    'Get Transaction Success': (transfer: TransferItem) => ({transfer}),
    'Rollback Transaction': (transfer: TransferItem) => ({transfer}),
    'Get Income': (tableLazyLoadEvent: TableLazyLoadEvent) => ({tableLazyLoadEvent}),
    'Get Income Success': (items: Item[]) => ({items}),
    'Get Expenses': (tableLazyLoadEvent: TableLazyLoadEvent) => ({tableLazyLoadEvent}),
    'Get Expenses Success': (items: Item[]) => ({items}),
    'Get Credits': (tableLazyLoadEvent: TableLazyLoadEvent | undefined) => ({tableLazyLoadEvent}),
    'Get Credits Success': (credits: CreditItem[], totalCount: number, tableLazyLoadEvent: TableLazyLoadEvent | undefined) => ({credits, totalCount, tableLazyLoadEvent}),
    'Edit Account': (id: string) => ({id}),
  },
});
