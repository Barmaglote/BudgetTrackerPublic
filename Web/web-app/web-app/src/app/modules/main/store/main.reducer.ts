import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { MainActions as MA } from '../store/main.action';
import { BriefStatistics } from '../../item/models/brief-statistics';

export const mainFeatureKey = 'mainFeature';

export interface MainState {
  briefStatistics: BriefStatistics | undefined,
}

export const initialState: MainState = {
  briefStatistics: undefined,
};

export const selectMainFeature = createFeatureSelector<MainState>(mainFeatureKey);

export const mainReducer = createReducer(
  initialState,
  on(MA.getBriefStatisticsSuccess, (state, { briefStatistics }) => ({ ...state, briefStatistics })),
  on(MA.cleanStore, () => ({ ...initialState })),
);
