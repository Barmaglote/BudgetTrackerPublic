import { createSelector } from '@ngrx/store';
import { selectReportsFeature as getState } from './reports.reducer';

const regularPayments = createSelector(getState, (state) => state.regularPayments);
const credits = createSelector(getState, (state) => state.credits);

export const ReportsSelectors = {
  regularPayments,
  credits
};
