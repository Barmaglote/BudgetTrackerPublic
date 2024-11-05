import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType, } from '@ngrx/effects';
import { UpgradeActions as UA } from './upgrade.action';
import { EMPTY, catchError, exhaustMap, map, of, tap } from 'rxjs';
import { Router } from '@angular/router';
import { UPGRADE_ROUTE, UPGRADE_ROUTE_MAP } from '../upgrade-routing.module';
import { AppError } from 'src/app/core/models';
import { UpgradeService } from '../services';
import { SettingsActions as SA } from '../../settings/store';


@Injectable()
export class UpgradeEffects {
  constructor(
    private actions$: Actions,
    private router: Router,
    private upgradeService: UpgradeService
  ) {}

  navigateToRoot$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(UA.navigateToRoot),
        tap(() => this.router.navigate([`/user`]))
      ),
      { dispatch: false }
  );

  navigateToPayment$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(UA.navigateToPayment),
        tap(() => this.router.navigate([`/${UPGRADE_ROUTE}/${UPGRADE_ROUTE_MAP.payment}`]))
      ),
      { dispatch: false }
  );

  paymentIsFailed$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(UA.paymentIsFailed),
        tap(() => this.router.navigate([`/${UPGRADE_ROUTE}/${UPGRADE_ROUTE_MAP.paymentFailed}`]))
      ),
      { dispatch: false }
  );

  paymentIsSuccessfull$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UA.paymentIsSuccessfull),
      exhaustMap(({planId, paymentMethodId, email, userId, provider}) => {
        if (!planId || !paymentMethodId || !email) {
          return EMPTY;
        }

        return this.upgradeService.subscribe(planId, paymentMethodId, email, userId, provider).pipe(
          tap(() => this.router.navigate([`/${UPGRADE_ROUTE}/${UPGRADE_ROUTE_MAP.paymentSuccess}`])),
        )
      })
    ),
    { dispatch: false }
  );

  unsubscribe$ = createEffect(() =>
  this.actions$.pipe(
    ofType(UA.unsubscribe),
    exhaustMap(({subscribtionId, userId, provider}) => {
      if (!subscribtionId || !userId) {
        return EMPTY;
      }

      return this.upgradeService.unsubscribe(userId, subscribtionId, provider).pipe(
        tap(() => this.router.navigate([`/${UPGRADE_ROUTE}/${UPGRADE_ROUTE_MAP.unsubscribe}`])),
      )
    })
  ),
  { dispatch: false }
  );
}

function throwError(msg: string) {
  throw new AppError(msg, true, {
    severity: 'error', life: 1000
  });
}
