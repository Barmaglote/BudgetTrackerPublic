import { createSelector } from '@ngrx/store';
import { selectPlanningFeature as getState } from './planning.reducer';

const payments = createSelector(getState, (state) => state.payments);
const year = createSelector(getState, (state) => state.year);
const month = createSelector(getState, (state) => state.month);
const plannedItems = createSelector(getState, (state) => state.plannedItems);

export const PlanningSelectors = {
  payments,
  year,
  month,
  plannedItems
};
