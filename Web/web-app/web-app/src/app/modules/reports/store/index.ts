import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { ReportsEffects } from './reports.effect';
import { reportsFeatureKey, reportsReducer } from './reports.reducer';

const REPORTS_EFFECTS = [
  ReportsEffects
];

export const REPORTS_PROVIDERS = [
  provideState({ name: reportsFeatureKey, reducer: reportsReducer }),
  provideEffects(REPORTS_EFFECTS),
];

export * from './reports.action';
export * from './reports.effect';
export * from './reports.reducer';
export * from './reports.selector';
