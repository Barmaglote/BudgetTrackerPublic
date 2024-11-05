import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { Item } from '../models';
import { ItemActions as IA } from '../store/item.action';
import { TableLazyLoadEvent } from 'primeng/table';
import { StatisticsByCategory } from 'src/app/models/statistics-by-category';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';

export const itemFeatureKey = 'itemFeature';

export interface ItemState {
  items: Item[] | undefined,
  statisticsByCategory: StatisticsByCategory[] | undefined,
  statisticsByDate: StatisticsByDate[] | undefined,
  tableLazyLoadEvent: TableLazyLoadEvent | undefined,
  totalCount: number | undefined,
  item: Item | undefined,
}

export const initialState: ItemState = {
  items: undefined,
  statisticsByCategory: undefined,
  statisticsByDate: undefined,
  tableLazyLoadEvent: undefined,
  totalCount: undefined,
  item: undefined,
};

export const selectItemFeature = createFeatureSelector<ItemState>(itemFeatureKey);

export const itemReducer = createReducer(
  initialState,
  on(IA.getItemsSuccess, (state, { items, totalCount, tableLazyLoadEvent }) => ({ ...state, items: updateItems(items), totalCount, tableLazyLoadEvent })),
  on(IA.getStatisticsByCategorySuccess, (state, { statisticsByCategory }) => ({ ...state, statisticsByCategory })),
  on(IA.getStatisticsByDateSuccess, (state, { statisticsByDate }) => ({ ...state, statisticsByDate })),
  on(IA.getItemSuccess, (state, { item }) => ({ ...state, item })),
  on(IA.getItem, (state) => ({ ...state, item: undefined })),
  on(IA.cleanStore, () => ({ ...initialState })),
);

function updateItems(items: Item[]){
  return items ? items.map(x => {
    let item =  new Item(x.category, x.quantity, x.date, x.comment, x.id, x.idString, x.isRegular, x.accountId, x.transactionId);
    return item;
    }) : [];
}
