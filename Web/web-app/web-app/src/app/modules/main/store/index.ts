import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { MainEffects } from './main.effect';
import { mainFeatureKey, mainReducer } from './main.reducer';

const MAIN_EFFECTS = [
  MainEffects
];

export const MAIN_PROVIDERS = [
  provideState({ name: mainFeatureKey, reducer: mainReducer }),
  provideEffects(MAIN_EFFECTS),
];

export * from './main.action';
export * from './main.effect';
export * from './main.reducer';
export * from './main.selector';
