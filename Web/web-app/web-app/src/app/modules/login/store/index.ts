import { provideEffects } from '@ngrx/effects';
import { provideState } from '@ngrx/store';
import { LoginEffects } from './login.effect';
import { loginFeatureKey, loginReducer } from './login.reducer';

const LOGIN_EFFECTS = [
  LoginEffects
];

export const LOGIN_PROVIDERS = [
  provideState({ name: loginFeatureKey, reducer: loginReducer }),
  provideEffects(LOGIN_EFFECTS),
];

export * from './login.action';
export * from './login.effect';
export * from './login.reducer';
export * from './login.selector';
