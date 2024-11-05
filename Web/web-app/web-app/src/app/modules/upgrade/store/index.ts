import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { UpgradeEffects } from './upgrade.effect';
import { upgradeFeatureKey, upgradeReducer } from './upgrade.reducer';

const UPGRADE_EFFECTS = [
  UpgradeEffects
];

export const UPGRADE_PROVIDERS = [
  provideState({ name: upgradeFeatureKey, reducer: upgradeReducer }),
  provideEffects(UPGRADE_EFFECTS),
];

export * from './upgrade.action';
export * from './upgrade.effect';
export * from './upgrade.reducer';
export * from './upgrade.selector';
