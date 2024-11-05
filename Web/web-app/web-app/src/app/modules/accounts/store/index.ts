import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { AccountsEffects } from './accounts.effect';
import { accountsFeatureKey, accountsReducer } from './accounts.reducer';

const ACCOUNTS_EFFECTS = [
  AccountsEffects
];

export const ACCOUNTS_PROVIDERS = [
  provideState({ name: accountsFeatureKey, reducer: accountsReducer }),
  provideEffects(ACCOUNTS_EFFECTS),
];

export * from './accounts.action';
export * from './accounts.effect';
export * from './accounts.reducer';
export * from './accounts.selector';
