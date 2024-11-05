import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { PlanningActions as PA } from './planning.action';
import { PaymentInfo } from '../../credits/models/payment-info';
import { PlannedItem } from 'src/app/shared/models/planned-item';

export const planningFeatureKey = 'planning';

export interface PlanningState {
  payments: PaymentInfo[] | undefined,
  plannedItems: PlannedItem[] | undefined,
  year: number | undefined,
  month: number | undefined,
}

export const initialState: PlanningState = {
  payments: undefined,
  plannedItems: undefined,
  year: undefined,
  month: undefined
};

export const selectPlanningFeature = createFeatureSelector<PlanningState>(planningFeatureKey);

export const planningReducer = createReducer(
  initialState,
  on(PA.getNextPaymentsSuccess, (state, { payments }) => ({ ...state, payments })),
  on(PA.getPlansSuccess, (state, { plannedItems, year, month }) => ({ ...state, plannedItems, year, month })),
  on(PA.cleanStore, () => ({ ...initialState })),
);
