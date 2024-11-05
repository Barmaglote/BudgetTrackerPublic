import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { UserSettings } from 'src/app/models/user-settings';
import { SettingsActions as SA } from './settings.action';

export const settingsFeatureKey = 'settings';

export interface SettingsState {
  userSettings: UserSettings | undefined
}

export const initialState: SettingsState = {
  userSettings: undefined,
};

export const selectSettingsFeature = createFeatureSelector<SettingsState>(settingsFeatureKey);

export const settingsReducer = createReducer(
  initialState,
  on(SA.getSettingsSuccess, (state, { userSettings }) =>  ({ ...state, userSettings })),
  on(SA.saveSettingsSuccess, (state, { userSettings }) =>  ({ ...state, userSettings })),
  on(SA.cleanStore, () => ({ ...initialState })),
);
