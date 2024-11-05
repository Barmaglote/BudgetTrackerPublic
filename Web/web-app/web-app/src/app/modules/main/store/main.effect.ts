import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType, } from '@ngrx/effects';
import { MainActions as MA } from './main.action';
import { of, exhaustMap, map, catchError, tap } from 'rxjs';
import { AppError } from 'src/app/core/models';
import { TranslocoService } from '@ngneat/transloco';
import { StatisticsService } from '../../item/services';

@Injectable()
export class MainEffects {
  constructor(
    private actions$: Actions,
    private statisticsService: StatisticsService,
    private translateService: TranslocoService,
  ) {}

  getBriefStatistics$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(MA.getBriefStatistics),
        exhaustMap(() =>
          this.statisticsService.getBriefStatistics().pipe(
            map((items) => MA.getBriefStatisticsSuccess(items)),
            catchError((error) => of(MA.failure('exceptions.API_NOT_AVAILABLE', error)))
          )
        )
      ),
      { dispatch: true }
  );

  failure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(MA.failure),
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
