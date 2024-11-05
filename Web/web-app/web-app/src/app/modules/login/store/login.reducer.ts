import { createFeatureSelector, createReducer } from '@ngrx/store';
//import { LoginActions as LA } from '../store/login.action';

export const loginFeatureKey = 'loginFeature';

export interface LoginState {
}

export const initialState: LoginState = {
};

export const selectLoginFeature = createFeatureSelector<LoginState>(loginFeatureKey);

export const loginReducer = createReducer(
  initialState,
);

