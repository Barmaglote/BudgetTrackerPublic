import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { ReportsActions as RA } from '../store/reports.action';
import { RegularStatistics } from '../../item/models/regualar-payments';
import { CreditItem } from '../../credits/models/credit-item';
import { TableLazyLoadEvent } from 'primeng/table';
export const reportsFeatureKey = 'reportsFeature';

export interface ReportsState {
  regularPayments: RegularStatistics | undefined,
  credits: CreditItem[] | undefined,
  creditsTableLazyLoadEvent: TableLazyLoadEvent | undefined,
  totalCount: number | undefined,
}

export const initialState: ReportsState = {
  credits: undefined,
  regularPayments: undefined,
  creditsTableLazyLoadEvent: undefined,
  totalCount: undefined,
};

export const selectReportsFeature = createFeatureSelector<ReportsState>(reportsFeatureKey);

export const reportsReducer = createReducer(
  initialState,
  on(RA.getRegularPaymentsSuccess, (state, { regularPayments }) => ({ ...state, regularPayments })),
  on(RA.getCreditsSuccess, (state, { credits, totalCount, tableLazyLoadEvent }) => ({ ...state, credits, totalCount, tableLazyLoadEvent })),
);

