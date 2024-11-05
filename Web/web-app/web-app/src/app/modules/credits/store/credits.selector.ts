import { createSelector } from '@ngrx/store';
import { selectCreditsFeature as getState } from './credits.reducer';

const credits = createSelector(getState, (state) => state.credits);
const tableLazyLoadEvent = createSelector(getState, (state) => state.tableLazyLoadEvent);
const totalCount = createSelector(getState, (state) => state.totalCount);
const payments = createSelector(getState, (state) => state.payments);
const generalCreditsStatistics = createSelector(getState, (state) => state.generalCreditsStatistics);
const credit = createSelector(getState, (state) => state.credit);

export const CreditsSelectors = {
  credits,
  tableLazyLoadEvent,
  totalCount,
  payments,
  generalCreditsStatistics,
  credit
};
