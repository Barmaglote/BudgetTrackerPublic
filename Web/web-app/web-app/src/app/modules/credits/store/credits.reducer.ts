import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { CreditsActions as CA } from '../store/credits.action';
import { TableLazyLoadEvent } from 'primeng/table';
import { CreditItem } from '../models/credit-item';
import { PaymentInfo } from '../models/payment-info';
import { GeneralCreditsStatistics } from '../models/general-credits-statistics';

export const creditsFeatureKey = 'creditsFeature';

export interface CreditsState {
  credits: CreditItem[] | undefined,
  tableLazyLoadEvent: TableLazyLoadEvent | undefined,
  totalCount: number | undefined,
  payments: PaymentInfo[] | undefined,
  generalCreditsStatistics: GeneralCreditsStatistics | undefined,
  credit: CreditItem | undefined,
}

export const initialState: CreditsState = {
  credits: undefined,
  tableLazyLoadEvent: undefined,
  totalCount: undefined,
  payments: undefined,
  generalCreditsStatistics: undefined,
  credit: undefined,
};

export const selectCreditsFeature = createFeatureSelector<CreditsState>(creditsFeatureKey);

export const creditsReducer = createReducer(
  initialState,
  on(CA.getCreditsSuccess, (state, { credits, totalCount, tableLazyLoadEvent }) => ({ ...state, credits, totalCount, tableLazyLoadEvent })),
  on(CA.getNextPaymentsSuccess, (state, { payments }) => ({ ...state, payments })),
  on(CA.getGeneralCreditsStatisticsSuccess, (state, { generalCreditsStatistics }) => ({ ...state, generalCreditsStatistics })),
  on(CA.getCreditSuccess, (state, { credit }) => ({ ...state, credit })),
  on(CA.cleanStore, () => ({ ...initialState })),
);
