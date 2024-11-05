import { createFeatureSelector, createReducer, on } from '@ngrx/store';
import { AccountsActions as AA } from '../store/accounts.action';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { Item } from '../../item/models';
import { CreditItem } from '../../credits/models/credit-item';
export const accountsFeatureKey = 'accountsFeature';

export interface AccountsState {
  statisticsByDateIncome: StatisticsByDate[] | undefined,
  statisticsByDateExpenses: StatisticsByDate[] | undefined,
  accountItem: AccountItem | undefined,
  totalCount: number | undefined,
  transfers: TransferItem[] | undefined,
  transfer: TransferItem | undefined,
  income: Item[] | undefined,
  expenses: Item[] | undefined,
  credits: CreditItem[] | undefined,
}

export const initialState: AccountsState = {
  statisticsByDateIncome: undefined,
  statisticsByDateExpenses: undefined,
  accountItem: undefined,
  totalCount: undefined,
  transfers: undefined,
  transfer: undefined,
  income: undefined,
  expenses: undefined,
  credits: undefined,
};

export const selectAccountsFeature = createFeatureSelector<AccountsState>(accountsFeatureKey);

export const accountsReducer = createReducer(
  initialState,
  on(AA.getStatisticsByDateIncomeSuccess, (state, { statisticsByDate }) => ({ ...state, statisticsByDateIncome: statisticsByDate })),
  on(AA.getStatisticsByDateExpensesSuccess, (state, { statisticsByDate }) => ({ ...state, statisticsByDateExpenses: statisticsByDate })),
  on(AA.getAccountSuccess, (state, { accountItem }) => ({ ...state, accountItem })),
  on(AA.getTransactionsSuccess, (state, { transfers, totalCount }) => ({ ...state, transfers, totalCount })),
  on(AA.getTransactionSuccess, (state, { transfer }) => ({ ...state, transfer })),
  on(AA.cleanStore, () => ({ ...initialState })),
  on(AA.getIncomeSuccess, (state, { items }) => ({ ...state, income: items })),
  on(AA.getExpensesSuccess, (state, { items }) => ({ ...state, expenses: items })),
  on(AA.getCreditsSuccess, (state, { credits }) => ({ ...state, credits })),
);

