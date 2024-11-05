import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { CreditsEffects } from './credits.effect';
import { creditsFeatureKey, creditsReducer } from './credits.reducer';

const CREDITS_EFFECTS = [
  CreditsEffects
];

export const CREDITS_PROVIDERS = [
  provideState({ name: creditsFeatureKey, reducer: creditsReducer }),
  provideEffects(CREDITS_EFFECTS),
];

export * from './credits.action';
export * from './credits.effect';
export * from './credits.reducer';
export * from './credits.selector';
