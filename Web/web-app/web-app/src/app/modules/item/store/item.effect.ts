import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType, } from '@ngrx/effects';
import { ItemActions as IA } from './item.action';
import { ItemSelectors as IS } from './item.selector';
import { of, exhaustMap, map, catchError, tap, mergeMap, EMPTY } from 'rxjs';
import { HealthService, ItemService, StatisticsService } from '../services';
import { AppError } from 'src/app/core/models';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { ITEM_ROUTE_MAP } from '../item-routing.module';
import { Store, select } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { TransferService } from '../../accounts/services';

@Injectable()
export class ItemEffects {
  constructor(
    private actions$: Actions,
    private itemService: ItemService,
    private statisticsService: StatisticsService,
    private transferService: TransferService,
    private healthService: HealthService,
    private translateService: TranslocoService,
    private router: Router,
    private store: Store,
    private messageService: MessageService
  ) {}

  getItems$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(IA.getItems),
        exhaustMap(({tableLazyLoadEvent, area}) =>
          this.itemService.getItems(tableLazyLoadEvent, area).pipe(
            map(({items, totalCount}) => IA.getItemsSuccess(items, totalCount, tableLazyLoadEvent)),
            catchError((error) => of(IA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  getStatisticsByCategory$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(IA.getStatisticsByCategory),
        exhaustMap(({tableLazyLoadEvent, area}) =>
          this.statisticsService.getStatsByCategory(tableLazyLoadEvent, area).pipe(
            map((items) => IA.getStatisticsByCategorySuccess(items)),
            catchError((error) => of(IA.failure('exceptions.API_NOT_AVAILABLE', error)))
          )
        )
      ),
      { dispatch: true }
  );

  getStatisticsByDate$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(IA.getStatisticsByDate),
        exhaustMap(({tableLazyLoadEvent, area}) =>
          this.statisticsService.getStatsByDate(tableLazyLoadEvent, area).pipe(
            map((items) => IA.getStatisticsByDateSuccess(items)),
            catchError((error) => of(IA.failure('exceptions.API_NOT_AVAILABLE', error)))
          )
        )
      ),
      { dispatch: true }
  );

  deleteTransaction$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(IA.deleteTransaction),
        concatLatestFrom(() => [this.store.select(IS.tableLazyLoadEvent)]),
        exhaustMap(([{id}, tableLazyLoadEvent]) =>
          this.transferService.deleteItem(id).pipe(
            mergeMap(() => {
              if (tableLazyLoadEvent) {
                this.store.dispatch(IA.getItems(tableLazyLoadEvent, 'income'));
                this.store.dispatch(IA.getStatisticsByCategory(tableLazyLoadEvent, 'income'));
                this.store.dispatch(IA.getStatisticsByDate(tableLazyLoadEvent, 'income'));
                this.store.dispatch(IA.getItems(tableLazyLoadEvent, 'expenses'));
                this.store.dispatch(IA.getStatisticsByCategory(tableLazyLoadEvent, 'expenses'));
                this.store.dispatch(IA.getStatisticsByDate(tableLazyLoadEvent, 'expenses'));
              }
              return EMPTY;
            }),
            catchError((error) => of(IA.failure('exceptions.FAILED_TO_DELETE_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  healthError$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(IA.healthError),
        exhaustMap(() =>
          this.healthService.getError().pipe(
            map((result) => console.log(result)),
            catchError((error) => of(IA.failure('exceptions.API_NOT_AVAILABLE', error)))
          )
        )
      ),
      { dispatch: false }
  );

  storeItem$ = createEffect(() =>
    () =>
      this.actions$.pipe(
        ofType(IA.storeItem),
        exhaustMap(({item, area}) =>
          this.itemService.upsertItem(item, area).pipe(
            map((result) => {

              if (result) {
                this.translateService.selectTranslate('item.store.itemsIsStored').subscribe(msg => {
                  this.messageService.clear();
                  this.messageService.add({
                    severity: 'success',
                    summary: '',
                    detail: msg,
                  });
                }).unsubscribe();

                return IA.updateItemAndStatistics(area);
              } else {
                return IA.failure('exceptions.FAILED_TO_ADD_DATA', null);
              }
            }),
            tap(() => this.router.navigate([`user/${area}/${ITEM_ROUTE_MAP.root}`])),
            catchError((error) => of(IA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  updateItemAndStatistics$ = createEffect(() =>
    this.actions$.pipe(
      ofType(IA.updateItemAndStatistics),
      concatLatestFrom(() => this.store.pipe(select(IS.tableLazyLoadEvent))),
      mergeMap(([{ area }, tableLazyLoadEvent]) => {
        if (tableLazyLoadEvent) {
          this.store.dispatch(IA.getItems(tableLazyLoadEvent, area));
          this.store.dispatch(IA.getStatisticsByCategory(tableLazyLoadEvent, area));
          this.store.dispatch(IA.getStatisticsByDate(tableLazyLoadEvent, area));
        }
        return EMPTY;
      })
    ),
    { dispatch: false}
  );

  deleteItem$ = createEffect(() =>
    this.actions$.pipe(
      ofType(IA.deleteItem),
      exhaustMap(({item, area}) =>
      this.itemService.deleteItem(item, area).pipe(
        map((result) => {
          if (result) {
            return IA.updateItemAndStatistics(area);
          } else {
            return IA.failure('exceptions.FAILED_TO_DELETE_DATA', null);
          }
        }),
        catchError((error) => of(IA.failure('exceptions.FAILED_TO_DELETE_DATA', null)))
      )
    )
    ),
    { dispatch: true },
  );

  showInfo$ = createEffect(() =>
    this.actions$.pipe(
      ofType(IA.showInfo),
      tap(({item, area}) => this.router.navigate([`user/${area}/${ITEM_ROUTE_MAP.root}/${item.idString}`])),
    ),
    { dispatch: false },
  );

  showTransactionInfo$ = createEffect(() =>
    this.actions$.pipe(
      ofType(IA.showTransactionInfo),
      tap(({item}) => this.router.navigate([`user/accounts/info/${item.transactionId}`])),
    ),
    { dispatch: false },
  );

  getItem$ = createEffect(() =>
    this.actions$.pipe(
      ofType(IA.getItem),
      exhaustMap(({id, area}) =>
      this.itemService.getItem(id, area).pipe(
        map((item) => {
          if (item) {
            return IA.getItemSuccess(item);
          } else {
            return IA.failure('exceptions.FAILED_TO_GET_DATA', null);
          }
        }),
        catchError((error) => of(IA.failure('exceptions.FAILED_TO_GET_DATA', error)))
      ),
    )
    ),
    { dispatch: true },
  );

  openSettings$ = createEffect(() =>
    this.actions$.pipe(
      ofType(IA.openSettings),
      tap(({anchor}) =>  this.router.navigate([`user/settings/categories`])),
    ),
    { dispatch: false },
  );

  failure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(IA.failure),
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
}
