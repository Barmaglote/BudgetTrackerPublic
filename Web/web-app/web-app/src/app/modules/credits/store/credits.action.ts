import { createActionGroup, emptyProps } from '@ngrx/store';
import { TableLazyLoadEvent } from 'primeng/table';
import { CreditItem } from '../models/credit-item';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { PaymentInfo } from '../models/payment-info';
import { GeneralCreditsStatistics } from '../models/general-credits-statistics';

export const CreditsActions  = createActionGroup({
  source: 'Credits',
  events: {
    'Clean Store': emptyProps(),
    'Get Credits': (tableLazyLoadEvent: TableLazyLoadEvent | undefined) => ({tableLazyLoadEvent}),
    'Get Credits Success': (credits: CreditItem[], totalCount: number, tableLazyLoadEvent: TableLazyLoadEvent | undefined) => ({credits, totalCount, tableLazyLoadEvent}),
    'Failure': (text: string, error: any) => ({text, error}),
    'Store Item': (item: CreditItem) => ({item}),
    'Activate Credit': (id: string, accountId: string) => ({id, accountId}),
    'Delete Credit': (item: CreditItem) => ({item}),
    'Add Transfer': (transfer: TransferItem, creditId?: string) => ({transfer, creditId}),
    'Get Next Payments': emptyProps(),
    'Get Next Payments Success': (payments: PaymentInfo[]) => ({payments}),
    'Get General Credits Statistics': emptyProps(),
    'Get General Credits Statistics Success': (generalCreditsStatistics: GeneralCreditsStatistics) => ({generalCreditsStatistics}),
    'Show Info': (id: string) => ({id}),
    'Get Credit': (id: string | undefined) => ({id}),
    'Get Credit Success': (credit: CreditItem) => ({credit}),
  },
});
