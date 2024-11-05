import { createSelector } from '@ngrx/store';
import { selectSettingsFeature as getState } from './settings.reducer';

const userSettings = createSelector(getState, (state) => state.userSettings);

export const SettingsSelectors = {
  userSettings,
};
