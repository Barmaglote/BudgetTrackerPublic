import { createSelector } from '@ngrx/store';
import { selectMainFeature as getState } from './main.reducer';

const briefStatistics = createSelector(getState, (state) => state.briefStatistics);

export const MainSelectors = {
  briefStatistics
};
