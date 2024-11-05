import { createActionGroup, emptyProps } from '@ngrx/store';
import { Item } from '../models';
import { TableLazyLoadEvent } from 'primeng/table';
import { StatisticsByCategory } from 'src/app/models/statistics-by-category';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';

export const ItemActions  = createActionGroup({
  source: 'Item',
  events: {
    'Clean Store': emptyProps(),
    'Get Items': (tableLazyLoadEvent: TableLazyLoadEvent, area: string) => ({tableLazyLoadEvent, area}),
    'Get Items Success': (items: Item[], totalCount: number, tableLazyLoadEvent: TableLazyLoadEvent | undefined) => ({items, totalCount, tableLazyLoadEvent}),
    'Get Statistics By Category': (tableLazyLoadEvent: TableLazyLoadEvent, area: string) => ({tableLazyLoadEvent, area}),
    'Get Statistics By Category Success': (statisticsByCategory: StatisticsByCategory[]) => ({statisticsByCategory}),
    'Get Statistics By Date': (tableLazyLoadEvent: TableLazyLoadEvent, area: string) => ({tableLazyLoadEvent, area}),
    'Get Statistics By Date Success': (statisticsByDate: StatisticsByDate[]) => ({statisticsByDate}),
    'Health Error': emptyProps(),
    'Health Error Failure': (error: any) => ({error}),
    'Store Item': (item: Item, area: string) => ({item, area}),
    'Delete Item': (item: Item, area: string) => ({item, area}),
    'Open Settings': (anchor: string) => ({anchor}),
    'Update Item And Statistics': (area: string) => ({area}),
    'Show Info': (item: Item, area: string) => ({item, area}),
    'Show Transaction Info': (item: Item) => ({item}),
    'Failure': (text: string, error: any) => ({text, error}),
    'Get Item': (id: string, area: string) => ({id, area}),
    'Get Item Success': (item: Item) => ({item}),
    'Delete Transaction': (id: string) => ({id})
  },
});
