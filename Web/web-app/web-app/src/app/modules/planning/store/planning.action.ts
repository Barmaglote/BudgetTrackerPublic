import { createActionGroup, emptyProps } from '@ngrx/store';
import { PaymentInfo } from '../../credits/models/payment-info';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { PlannedItem } from 'src/app/shared/models/planned-item';
import { Item } from '../../item/models';
export const PlanningActions  = createActionGroup({
  source: 'Planning',
  events: {
    'Clean Store': emptyProps(),
    'Failure': (text: string, error: any) => ({text, error}),
    'Get Plans': (year?: number, month?: number) => ({year, month}),
    'Get Plans Success': (plannedItems: PlannedItem[], year?: number, month?: number) => ({plannedItems, year, month}),
    'Get Next Payments': emptyProps(),
    'Get Next Payments Success': (payments: PaymentInfo[]) => ({payments}),
    'Add Transfer': (transfer: TransferItem, creditId?: string) => ({transfer, creditId}),
    'Add Planned Item': (plannedItem: PlannedItem) => ({plannedItem}),
    'Delete Planned Item': (plannedItem: PlannedItem) => ({plannedItem}),
    'Store Item': (item: Item, area: string) => ({item, area}),
    'Store Item From Planned': (item: Item, area: string, id: string) => ({item, area, id}),
  },
});
