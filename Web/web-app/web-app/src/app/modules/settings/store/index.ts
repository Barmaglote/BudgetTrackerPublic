import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { SettingsEffects } from './settings.effect';
import { settingsFeatureKey, settingsReducer } from './settings.reducer';

const SETTINGS_EFFECTS = [
  SettingsEffects
];

export const SETTINGS_PROVIDERS = [
  provideState({ name: settingsFeatureKey, reducer: settingsReducer }),
  provideEffects(SETTINGS_EFFECTS),
];

export * from './settings.action';
export * from './settings.effect';
export * from './settings.reducer';
export * from './settings.selector';
