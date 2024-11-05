import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, tap } from 'rxjs';
import { PlanningActions as PA } from './planning.action';
import { PlanningSelectors as PS } from './planning.selector';

import { TranslocoService, translate } from '@ngneat/transloco';
import { CreditsService } from '../../credits/services';
import { AppError } from 'src/app/core/models';
import { TransferService } from '../../accounts/services';
import { MessageService } from 'primeng/api';
import { PlanningService } from '../services/planning-service';
import { Store } from '@ngrx/store';
import { ItemService } from '../../item/services';
import { Router } from '@angular/router';

@Injectable()
export class PlanningEffects {
  constructor(
    private actions$: Actions,
    private translateService: TranslocoService,
    private creditsService: CreditsService,
    private transferService: TransferService,
    private messageService: MessageService,
    private planningService: PlanningService,
    private itemService: ItemService,
    private store: Store
  ) {}

  getNextPayments$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PA.getNextPayments),
        exhaustMap(() =>
          this.creditsService.getNextPayments().pipe(
            map((payments) => PA.getNextPaymentsSuccess(payments)),
            catchError((error) => of(PA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  addTransfer$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PA.addTransfer),
        exhaustMap(({transfer, creditId}) => {
            return this.transferService.addItem(transfer, creditId).pipe(
              map(() => PA.getNextPayments()),
              tap(() => this.notify('credits.store.itemsIsStored', 'info')),
              catchError((error) => of(PA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
            )
          })
      ),
      { dispatch: true }
  );

  storeItem$ = createEffect(() =>
    () =>
      this.actions$.pipe(
        ofType(PA.storeItem),
        exhaustMap(({item, area}) =>
          this.itemService.upsertItem(item, area).pipe(
            tap(() => this.notify('credits.store.itemsIsStored', 'info')),
            catchError((error) => of(PA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: false }
  );

  storeItemFromPlanned$ = createEffect(() =>
    () =>
      this.actions$.pipe(
        ofType(PA.storeItemFromPlanned),
        concatLatestFrom(() => [this.store.select(PS.year), this.store.select(PS.month)]),
        exhaustMap(([{item, area, id}, year, month]) =>
          this.itemService.upsertItem(item, area, id).pipe(
            tap(() => this.notify('credits.store.itemsIsStored', 'info')),
            map(() => PA.getPlans(year, month)),
            catchError((error) => of(PA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  addPlannedItem$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PA.addPlannedItem),
        concatLatestFrom(() => [this.store.select(PS.year), this.store.select(PS.month)]),
        exhaustMap(([{plannedItem}, year, month]) => {
          return this.planningService.saveItem(plannedItem).pipe(
              map(() => PA.getPlans(year, month)),
              tap(() => this.notify('credits.store.itemsIsStored', 'info')),
              catchError((error) => of(PA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
            )
          })
      ),
      { dispatch: true }
  );

  deletePlannedItem$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PA.deletePlannedItem),
        concatLatestFrom(() => [this.store.select(PS.year), this.store.select(PS.month)]),
        exhaustMap(([{plannedItem}, year, month]) => {
          return this.planningService.deleteItem(plannedItem).pipe(
              map(() => PA.getPlans(year, month)),
              tap(() => this.notify('credits.store.itemsIsStored', 'info')),
              catchError((error) => of(PA.failure('exceptions.FAILED_TO_DELETE_DATA', error)))
            )
          })
      ),
      { dispatch: true }
  );

  getPlans$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PA.getPlans),
        exhaustMap(({year, month}) => {
            return this.planningService.getItems(year, month).pipe(
              map((plannedItems) => PA.getPlansSuccess(plannedItems, year, month)),
              catchError((error) => of(PA.failure('exceptions.FAILED_TO_GET_DATA', error)))
            )
          })
      ),
      { dispatch: true }
  );

  failure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(PA.failure),
        tap(({ text, error }) => {
          this.showError(text, error);
        }),
      ),
    { dispatch: false },
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
    this.translateService.selectTranslate(text).subscribe(value => {
      this.messageService.clear();
      this.messageService.add({
        severity,
        summary: '',
        detail: value,
      });
    }).unsubscribe();
  }
}
