import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { UpgradeActions as UA } from '../store/upgrade.action';
import { PaymentPlan } from '../models/payment-plan';

export const upgradeFeatureKey = 'upgradeFeature';

export interface UpgradeState {
  plan: PaymentPlan | undefined
}

export const initialState: UpgradeState = {
  plan: undefined
};

export const selectUpgradeFeature = createFeatureSelector<UpgradeState>(upgradeFeatureKey);

export const upgradeReducer = createReducer(
  initialState,
  on(UA.cleanStore, () => ({ ...initialState })),
  on(UA.navigateToPayment, (state, { plan }) => ({ ...state, plan })),
);
