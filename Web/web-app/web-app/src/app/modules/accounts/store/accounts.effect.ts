import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AccountsActions as AA } from './accounts.action';
import { SettingsActions as SA } from '../../settings/store/settings.action';
import { of, exhaustMap, map, catchError, tap } from 'rxjs';
import { TransferService } from '../services';
import { AppError } from 'src/app/core/models';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { ACCOUNTS_ROUTE, ACCOUNTS_ROUTE_MAP } from '../accounts-routing.module';
import { ItemService, StatisticsService } from '../../item/services';
import { SettingsService } from '../../settings/services';
import { TableLazyLoadEvent } from 'primeng/table';
import { Store } from '@ngrx/store';
import { CreditsService } from '../../credits/services';
import { SETTING_ROUTE, SETTING_ROUTE_MAP } from '../../settings/settings-routing.module';

@Injectable()
export class AccountsEffects {
  constructor(
    private actions$: Actions,
    private transferService: TransferService,
    private translateService: TranslocoService,
    private statisticsService: StatisticsService,
    private settingsService: SettingsService,
    private creditsService: CreditsService,
    private router: Router,
    private store: Store,
    private itemService: ItemService
  ) {}

  addTransfer$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.addTransfer),
        exhaustMap(({ transfer }) => {
          return this.transferService.addItem(transfer).pipe(
            map((result) => SA.getSettings()),
            catchError((error) => {
              console.log("AddTransfer error", error);
              return of(AA.failure('exceptions.FAILED_TO_ADD_DATA', error))
            })
          );
        })
      ),
    { dispatch: true }
  );

  getStatisticsByDateIncome$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getStatisticsByDateIncome),
        exhaustMap(({ tableLazyLoadEvent }) =>
          this.statisticsService
            .getStatsByDate(tableLazyLoadEvent, 'income')
            .pipe(
              map((items) => AA.getStatisticsByDateIncomeSuccess(items)),
              catchError((error) =>
                of(AA.failure('exceptions.API_NOT_AVAILABLE', error))
              )
            )
        )
      ),
    { dispatch: true }
  );

  getTransactions$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getTransactions),
        exhaustMap(({ tableLazyLoadEvent }) =>
          this.transferService.getItems(tableLazyLoadEvent).pipe(
            map(({ items, totalCount }) =>
              AA.getTransactionsSuccess(items, totalCount)
            ),
            catchError((error) =>
              of(AA.failure('exceptions.API_NOT_AVAILABLE', error))
            )
          )
        )
      ),
    { dispatch: true }
  );

  getIncome$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getIncome),
        exhaustMap(({ tableLazyLoadEvent }) => {
          return this.itemService.getItems(tableLazyLoadEvent, 'income').pipe(
            map(({ items }) => AA.getIncomeSuccess(items)),
            catchError((error) =>
              of(AA.failure('exceptions.FAILED_TO_ADD_DATA', error))
            )
          );
        })
      ),
    { dispatch: true }
  );

  getExpenses$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getExpenses),
        exhaustMap(({ tableLazyLoadEvent }) => {
          return this.itemService.getItems(tableLazyLoadEvent, 'expenses').pipe(
            map(({ items }) => AA.getExpensesSuccess(items)),
            catchError((error) =>
              of(AA.failure('exceptions.FAILED_TO_ADD_DATA', error))
            )
          );
        })
      ),
    { dispatch: true }
  );

  getAccount$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getAccount),
        exhaustMap(({ id }) =>
          this.settingsService.getAccount(id).pipe(
            map((items) => AA.getAccountSuccess(items)),
            catchError((error) =>
              of(AA.failure('exceptions.FAILED_TO_GET_DATA', error))
            )
          )
        )
      ),
    { dispatch: true }
  );

  getStatisticsByDateExpenses$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getStatisticsByDateExpenses),
        exhaustMap(({ tableLazyLoadEvent }) =>
          this.statisticsService
            .getStatsByDate(tableLazyLoadEvent, 'expenses')
            .pipe(
              map((items) => AA.getStatisticsByDateExpensesSuccess(items)),
              catchError((error) =>
                of(AA.failure('exceptions.API_NOT_AVAILABLE', error))
              )
            )
        )
      ),
    { dispatch: true }
  );

  getTransaction$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getTransaction),
        exhaustMap(({ id }) =>
          this.transferService.getItem(id).pipe(
            map((transaction) => AA.getTransactionSuccess(transaction)),
            catchError((error) =>
              of(AA.failure('exceptions.Failed_To_Get_Data', error))
            )
          )
        )
      ),
    { dispatch: true }
  );

  rollbackTransaction$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.rollbackTransaction),
        exhaustMap(({ transfer }) =>
          this.transferService.deleteItem(transfer?.transactionId || '').pipe(
            map(() => SA.getSettings()),
            tap(() => {
              var initial = {} as TableLazyLoadEvent;
              initial.first = 0;
              initial.rows = 50;
              initial.sortField = 'day';
              initial.sortOrder = 1;

              this.store.dispatch(AA.getStatisticsByDateIncome(initial));
              this.store.dispatch(AA.getStatisticsByDateExpenses(initial));
            }),
            catchError((error) => of(AA.failure('exceptions.Failed_To_Get_Data', error)))
          )
        )
      ),
    { dispatch: true }
  );

  getCredits$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.getCredits),
        exhaustMap(({ tableLazyLoadEvent }) =>
          this.creditsService.getItems(tableLazyLoadEvent).pipe(
            map(({ items, totalCount }) =>
              AA.getCreditsSuccess(items, totalCount, tableLazyLoadEvent)
            ),
            catchError((error) =>
              of(AA.failure('exceptions.FAILED_TO_ADD_DATA', error))
            )
          )
        )
      ),
    { dispatch: true }
  );

  deleteAccount$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.deleteAccount),
        exhaustMap(({ id }) => {
          if (!id) {
            return of(AA.failure('Failed_To_Delete_Data', null));
          }

          return this.settingsService.deleteAccount(id).pipe(
            map(() => SA.getSettings()),
            tap(() => {
              var initial = {} as TableLazyLoadEvent;
              initial.first = 0;
              initial.rows = 50;
              initial.sortField = 'day';
              initial.sortOrder = 1;

              this.store.dispatch(AA.getStatisticsByDateIncome(initial));
              this.store.dispatch(AA.getStatisticsByDateExpenses(initial));
            }),
            catchError((error) =>
              of(SA.raiseException('Failed_To_Delete_Data', error))
            )
          );
        })
      ),
    { dispatch: true }
  );

  showTransactionInfo$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.showTransactionInfo),
        tap(({ transfer }) => {
          this.router.navigate([
            `${ACCOUNTS_ROUTE}/${ACCOUNTS_ROUTE_MAP.root}/info/${transfer.transactionId}`,
          ]);
        })
      ),
    { dispatch: false }
  );

  editAccount$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.editAccount),
        tap(({ id }) => {
          this.router.navigate([
            `/user/${SETTING_ROUTE}/${SETTING_ROUTE_MAP.accounts}`,
          ], { queryParams: { account: id } });
        })
      ),
    { dispatch: false }
  );

  showAccountInfo$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.showAccountInfo),
        tap(({ id }) => {
          this.router.navigate([
            `${ACCOUNTS_ROUTE}/${ACCOUNTS_ROUTE_MAP.root}/${id}`,
          ]);
        })
      ),
    { dispatch: false }
  );

  failure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AA.failure),
        tap(({ text, error }) => {
          this.showError(text, error);
        })
      ),
    { dispatch: false }
  );

  private showError(text: string, error: any): void {
    this.translateService.selectTranslate(text).subscribe((msg) => {
      throw new AppError(msg, true, {
        severity: 'error',
        life: 1000,
      });
    }).unsubscribe();
  }
}
