import { createSelector } from '@ngrx/store';
import { selectItemFeature as getState } from './item.reducer';

const items = createSelector(getState, (state) => state.items);
const statisticsByCategory = createSelector(getState, (state) => state.statisticsByCategory);
const statisticsByDate = createSelector(getState, (state) => state.statisticsByDate);
const tableLazyLoadEvent = createSelector(getState, (state) => state.tableLazyLoadEvent);
const totalCount = createSelector(getState, (state) => state.totalCount);
const item = createSelector(getState, (state) => state.item);

export const ItemSelectors = {
  items,
  statisticsByCategory,
  statisticsByDate,
  tableLazyLoadEvent,
  totalCount,
  item
};
