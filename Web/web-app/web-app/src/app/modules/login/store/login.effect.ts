import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType, } from '@ngrx/effects';
import { LoginActions as LA } from './login.action';
import { of, exhaustMap, map, catchError, tap, EMPTY, switchMap } from 'rxjs';
import { LoginService } from '../services';
import { AppError } from 'src/app/core/models';
import { TranslocoService } from '@ngneat/transloco';
import { Router } from '@angular/router';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from '../login-routing.module';
import { AuthenticationService } from 'src/app/core/services';
import { ACCOUNTS_ROUTE, ACCOUNTS_ROUTE_MAP } from '../../accounts/accounts-routing.module';

@Injectable()
export class LoginEffects {
  constructor(
    private actions$: Actions,
    private loginService: LoginService,
    private translateService: TranslocoService,
    private authenticationService: AuthenticationService,
    private router: Router
  ) {}

  signUp$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LA.signUp),
      exhaustMap(({title, email, password, recaptcha}) =>
      this.loginService.signUp(title, email, password, recaptcha).pipe(
        map((result: boolean) => {
          if (result) {
            return LA.showSignUpConfirmation();
          } else {
            return LA.failure('exceptions.FAILED_TO_DELETE_DATA', null);
          }
        }),
        catchError((error) => of(LA.failure('exceptions.FAILED_TO_DELETE_DATA', null)))
      )
    )
    ),
    { dispatch: true },
  );

  restoreRequest$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LA.restoreRequest),
      exhaustMap(({login, recaptcha}) =>
      this.loginService.requestrestore(login, recaptcha).pipe(
        map((result) => {
          if (result.status === 'success') {
            return LA.showRestoreRequestConfirmation();
          } else {
            return LA.failure('exceptions.FAILED_TO_GET_DATA', null);
          }
        }),
        catchError((error) => of(LA.failure('exceptions.FAILED_TO_GET_DATA', null)))
      )
    )),
    { dispatch: true },
  );

  restore$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LA.restore),
      exhaustMap(({login, password, token}) =>
      this.loginService.restore(password, token, login).pipe(
        map((result) => {
          if (result.status === 'success') {
            return LA.showRestoreConfirmation();
          } else {
            return LA.failure('exceptions.FAILED_TO_GET_DATA', null);
          }
        }),
        catchError((error) => of(LA.failure('exceptions.FAILED_TO_GET_DATA', null)))
      )
    )),
    { dispatch: true },
  );

  changePasswordRequest$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LA.changePasswordRequest),
      exhaustMap(({login, password, newpassword}) =>
      this.loginService.changePasswordRequest(login, password, newpassword).pipe(
        map((result) => {
          if (result.status === 'success') {
            return LA.changePasswordRequestConfirmation();
          } else {
            return LA.failure('exceptions.FAILED_TO_CHANGE_PASSWORD', null);
          }
        }),
        catchError((error) => of(LA.failure('exceptions.FAILED_TO_GET_DATA', null)))
      )
    )),
    { dispatch: true },
  );

  changePasswordRequestConfirmation$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LA.changePasswordRequestConfirmation),
        tap(() => {
          this.router.navigate([`${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.passwordRequestSent}`])
        }),
      ),
    { dispatch: false },
  );


  showRestoreRequestConfirmation$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LA.showRestoreRequestConfirmation),
        tap(() => {
          this.router.navigate([`${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.restoreSent}`])
        }),
      ),
    { dispatch: false },
  );

  showRestoreConfirmation$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LA.showRestoreConfirmation),
        tap(() => {
          this.router.navigate([`${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.restoreConfirmed}`])
        }),
      ),
    { dispatch: false },
  );

  confirmation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LA.confirmation),
      exhaustMap(({login, token}) =>
      this.loginService.confirm(login, token).pipe(
        map((result: boolean) => {
          if (result) {
            return LA.showSignUpConfirmation();
          } else {
            return LA.failure('exceptions.FAILED_TO_DELETE_DATA', null);
          }
        }),
        catchError((error) => of(LA.failure('exceptions.FAILED_TO_DELETE_DATA', null)))
      )
    )
    ),
    { dispatch: true },
  );

  signIn$ = createEffect(() =>
    this.actions$.pipe(
      ofType(LA.signIn),
      exhaustMap(({login, password, recaptcha}) =>
        this.loginService.signIn(login, password, recaptcha).pipe(
          switchMap((result: any) => {
            console.log("signIn", {result});

            if (result?.status === 'success') {
              this.authenticationService.signUser({
                provider: 'budgettracker',
                id: result.user.id,
                email: result.user.login,
                name: result.user.username,
                photoUrl: '',
                firstName: '',
                lastName: '',
                authToken: result.accessToken,
                idToken: result.accessToken,
                authorizationCode: result.refreshToken,
                response: ''
              });

              return this.authenticationService.updateCSRF().pipe(
                map(() => LA.showStartPage()),
                catchError(() => of(LA.failure('exceptions.FAILED_TO_SIGN_IN', null)))
              );
            } else {
              return of(LA.failure('exceptions.FAILED_TO_SIGN_IN', null));
            }
          }),
          catchError((error) => {
            console.log(error);
            return of(LA.failure('exceptions.FAILED_TO_SIGN_IN', null))
          })
        )
      )
    ),
    { dispatch: true },
  );

  showStartPage$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LA.showStartPage),
        tap(() => this.router.navigate([`${ACCOUNTS_ROUTE}/${ACCOUNTS_ROUTE_MAP.root}`])),
      ),
    { dispatch: false },
  );

  showSignUpConfirmation$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LA.showSignUpConfirmation),
        tap(() => {
          this.router.navigate([`${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.signupConfirmed}`])
        }),
      ),
    { dispatch: false },
  );

  failure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(LA.failure),
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
