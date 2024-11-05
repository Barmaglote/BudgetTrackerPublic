import { createSelector } from '@ngrx/store';
import { selectAccountsFeature as getState } from './accounts.reducer';

const statisticsByDateIncome = createSelector(getState, (state) => state.statisticsByDateIncome);
const statisticsByDateExpenses = createSelector(getState, (state) => state.statisticsByDateExpenses);
const accountItem = createSelector(getState, (state) => state.accountItem);
const transfers = createSelector(getState, (state) => state.transfers);
const totalCount = createSelector(getState, (state) => state.totalCount);
const transfer = createSelector(getState, (state) => state.transfer);
const income = createSelector(getState, (state) => state.income);
const expenses = createSelector(getState, (state) => state.expenses);
const credits = createSelector(getState, (state) => state.credits);

export const AccountsSelectors = {
  statisticsByDateIncome,
  statisticsByDateExpenses,
  accountItem,
  transfers,
  totalCount,
  transfer,
  income,
  expenses,
  credits
};
