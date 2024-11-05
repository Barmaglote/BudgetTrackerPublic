import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType, } from '@ngrx/effects';
import { CreditsActions as CA } from './credits.action';
import { CreditsSelectors as CS } from './credits.selector';
import { of, exhaustMap, map, catchError, tap, EMPTY, mergeMap } from 'rxjs';
import { AppError } from 'src/app/core/models';
import { TranslocoService } from '@ngneat/transloco';
import { CreditsService } from '../services';
import { MessageService } from 'primeng/api';
import { Store } from '@ngrx/store';
import { StatisticsService, TransferService } from '../../accounts/services';
import { Router } from '@angular/router';
import { CREDITS_ROUTE, CREDITS_ROUTE_MAP } from '../credits-routing.module';

@Injectable()
export class CreditsEffects {
  constructor(
    private actions$: Actions,
    private translateService: TranslocoService,
    private creditsService: CreditsService,
    private statisticsService: StatisticsService,
    private messageService: MessageService,
    private transferService: TransferService,
    private store: Store,
    private router: Router
  ) {}

  getCredits$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.getCredits),
        exhaustMap(({tableLazyLoadEvent}) =>
          this.creditsService.getItems(tableLazyLoadEvent).pipe(
            map(({items, totalCount}) => CA.getCreditsSuccess(items, totalCount, tableLazyLoadEvent)),
            catchError((error) => of(CA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  getNextPayments$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.getNextPayments),
        exhaustMap(() =>
          this.creditsService.getNextPayments().pipe(
            map((payments) => CA.getNextPaymentsSuccess(payments)),
            catchError((error) => of(CA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  deleteCredit$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.deleteCredit),
        concatLatestFrom(() => [this.store.select(CS.tableLazyLoadEvent)]),
        exhaustMap(([{item}, tableLazyLoadEvent]) => {
          if (!item) {
            return EMPTY;
          }
          return this.creditsService.deleteItem(item).pipe(
            mergeMap((isAdded) => [CA.getCredits(tableLazyLoadEvent || undefined), CA.getGeneralCreditsStatistics()]),
            tap(() => this.notify('credits.store.itemsIsStored', 'info')),
            catchError((error) => of(CA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        })
      ),
      { dispatch: true }
  );

  storeItem$ = createEffect(() =>
    () =>
      this.actions$.pipe(
        ofType(CA.storeItem),
        concatLatestFrom(() => [this.store.select(CS.tableLazyLoadEvent)]),
        exhaustMap(([{item}, tableLazyLoadEvent]) =>
          this.creditsService.upsertItem(item).pipe(
            mergeMap((isAdded) => [
              CA.getCredits(tableLazyLoadEvent || undefined),
              CA.getNextPayments(),
              CA.getGeneralCreditsStatistics()
            ]),
            tap(() => this.notify('credits.store.itemsIsStored', 'info')),
            catchError((error) => of(CA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  addTransfer$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.addTransfer),
        concatLatestFrom(() => [this.store.select(CS.tableLazyLoadEvent)]),
        exhaustMap(([{transfer, creditId}, tableLazyLoadEvent]) => {
            return this.transferService.addItem(transfer, creditId).pipe(
              mergeMap((isAdded) => [
                CA.getCredits(tableLazyLoadEvent || undefined),
                CA.getNextPayments(),
                CA.getGeneralCreditsStatistics(),
                CA.getCredit(creditId)
              ]),
              tap(() => this.notify('credits.store.itemsIsStored', 'info')),
              catchError((error) => of(CA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
            )
          })
      ),
      { dispatch: true }
  );

  activateCredit$ = createEffect(() =>
    () =>
      this.actions$.pipe(
        ofType(CA.activateCredit),
        concatLatestFrom(() => [this.store.select(CS.tableLazyLoadEvent)]),
        exhaustMap(([{id, accountId}, tableLazyLoadEvent]) =>
          this.creditsService.activate(id, accountId).pipe(
            mergeMap((isAdded) => [
              CA.getCredits(tableLazyLoadEvent || undefined),
              CA.getNextPayments(),
              CA.getGeneralCreditsStatistics()
            ]),
            tap(() => this.notify('credits.store.itemsIsStored', 'info')),
            catchError((error) => of(CA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  showInfo$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.showInfo),
        tap(({ id }) => {
          this.router.navigate([`${CREDITS_ROUTE}/${CREDITS_ROUTE_MAP.root}/${id}`])
        }),
      ),
    { dispatch: false },
  );

  getCredit$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.getCredit),
        exhaustMap(({id}) => {
          if (!id) { return EMPTY; }
          return this.creditsService.getItem(id).pipe(
            map((items) => CA.getCreditSuccess(items)),
            catchError((error) => of(CA.failure('exceptions.FAILED_TO_GET_DATA', error)))
          )
        })
      ),
      { dispatch: true }
  );

  failure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.failure),
        tap(({ text, error }) => {
          this.showError(text, error);
        }),
      ),
    { dispatch: false },
  );

  getGeneralCreditsStatistics$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CA.getGeneralCreditsStatistics),
        exhaustMap(() =>
          this.statisticsService.getGeneralCreditsStatistics().pipe(
            map((generalCreditsStatistics) => CA.getGeneralCreditsStatisticsSuccess(generalCreditsStatistics)),
            catchError((error) => of(CA.failure('exceptions.Failed_To_Get_Data', error)))
          )
        )
      ),
      { dispatch: true }
  );

  private showError(text: string, error: any): void {
    console.log("ERROR", error);
    this.translateService.selectTranslate(text).subscribe(msg => {
      throw new AppError(msg, true, {
        severity: 'error', life: 1000
      });
    }).unsubscribe();
  }

  private notify(text: string, severity: string): void {
    this.translateService.selectTranslate(text).subscribe(detail => {
      this.messageService.clear();
      this.messageService.add({
        severity,
        summary: '',
        detail,
      });
    }).unsubscribe();
  }
}
