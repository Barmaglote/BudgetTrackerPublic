import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { PlanningEffects } from './planning.effect';
import { planningFeatureKey, planningReducer } from './planning.reducer';

const PLANNING_EFFECTS = [
  PlanningEffects
];

export const PLANNING_PROVIDERS = [
  provideState({ name: planningFeatureKey, reducer: planningReducer }),
  provideEffects(PLANNING_EFFECTS),
];

export * from './planning.action';
export * from './planning.effect';
export * from './planning.reducer';
export * from './planning.selector';
