import { createActionGroup, emptyProps } from '@ngrx/store';
import { RegularStatistics } from '../../item/models/regualar-payments';
import { CreditItem } from '../../credits/models/credit-item';
import { TableLazyLoadEvent } from 'primeng/table';

export const ReportsActions = createActionGroup({
  source: 'Reports',
  events: {
    'Get Report': emptyProps(),
    'Failure': (text: string, error: any) => ({text, error}),
    'Get Regular Payments': (year: number) => ({year}),
    'Get Regular Payments Success': (regularPayments: RegularStatistics) => ({regularPayments}),
    'Get Credits': (tableLazyLoadEvent: TableLazyLoadEvent | undefined) => ({tableLazyLoadEvent}),
    'Get Credits Success': (credits: CreditItem[], totalCount: number, tableLazyLoadEvent: TableLazyLoadEvent | undefined) => ({credits, totalCount, tableLazyLoadEvent}),
  },
});
