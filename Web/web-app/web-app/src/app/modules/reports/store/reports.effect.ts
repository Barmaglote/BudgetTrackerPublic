import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType, } from '@ngrx/effects';
import { ReportsActions as RA } from './reports.action';
import { catchError, exhaustMap, map, of, tap } from 'rxjs';
import { AppError } from 'src/app/core/models';
import { TranslocoService } from '@ngneat/transloco';
import { StatisticsService } from '../../item/services';
import { CreditsService } from '../../credits/services';

@Injectable()
export class ReportsEffects {
  constructor(
    private actions$: Actions,
    private translateService: TranslocoService,
    private statisticsService: StatisticsService,
    private creditsService: CreditsService
  ) {}

  failure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(RA.failure),
        tap(({ text, error }) => {
          this.showError(text, error);
        }),
      ),
    { dispatch: false },
  );

  getCredits$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(RA.getCredits),
        exhaustMap(({tableLazyLoadEvent}) =>
          this.creditsService.getItems(tableLazyLoadEvent).pipe(
            map(({items, totalCount}) => RA.getCreditsSuccess(items, totalCount, tableLazyLoadEvent)),
            catchError((error) => of(RA.failure('exceptions.FAILED_TO_ADD_DATA', error)))
          )
        )
      ),
      { dispatch: true }
  );

  getRegularPayments$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(RA.getRegularPayments),
        exhaustMap(({year}) =>
          this.statisticsService.getRegularPayments(year).pipe(
            map((settings) => RA.getRegularPaymentsSuccess(settings)),
            catchError((error) => of(RA.failure('exceptions.FAILED_TO_GET_DATA', error)))
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
}
