import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { ItemEffects } from './item.effect';
import { itemFeatureKey, itemReducer } from './item.reducer';

const ITEM_EFFECTS = [
  ItemEffects
];

export const ITEM_PROVIDERS = [
  provideState({ name: itemFeatureKey, reducer: itemReducer }),
  provideEffects(ITEM_EFFECTS),
];

export * from './item.action';
export * from './item.effect';
export * from './item.reducer';
export * from './item.selector';
