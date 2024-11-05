
import { createSelector } from '@ngrx/store';
import { selectUpgradeFeature as getState } from './upgrade.reducer';

const plan = createSelector(getState, (state) => state.plan);

export const UpgradeSelectors = {
  plan
};
