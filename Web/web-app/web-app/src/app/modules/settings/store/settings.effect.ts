import { Injectable } from '@angular/core';
import { Actions, concatLatestFrom, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, tap } from 'rxjs';
import { SettingsActions as SA } from './settings.action';
import { SettingsSelectors as SS } from './settings.selector';
import { SettingsService } from '../services';
import { AppError } from 'src/app/core/models';
import { translate } from '@ngneat/transloco';
import { MessageService } from 'primeng/api';
import { Store } from '@ngrx/store';

@Injectable()
export class SettingsEffects {
  constructor(
    private actions$: Actions,
    private settingsService: SettingsService,
    private messageService: MessageService,
    private store: Store
  ) {}

  getSettings$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.getSettings),
        exhaustMap(() =>
          this.settingsService.getUserSettings().pipe(
            map((settings) => SA.getSettingsSuccess(settings)),
            catchError((error) => {
              console.log("getSettings catchError", error)
              return of(SA.raiseException("Failed_To_Get_Settings", error))
            })
          )
        )
      ),
      { dispatch: true }
  );

  saveCategories$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.saveCategories),
        concatLatestFrom(() => [this.store.select(SS.userSettings)]),
        switchMap(([{categories}, userSettings]) => {
          if (!userSettings) {
            return of(SA.raiseException("Failed_To_Get_Settings", null));
          }

          let settings = JSON.parse(JSON.stringify(userSettings));
          settings.categories = categories;

          return of(SA.saveSettings(settings));
        })
      ),
      { dispatch: true }
  );

  saveTemplates$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.saveTemplates),
        concatLatestFrom(() => [this.store.select(SS.userSettings)]),
        switchMap(([{templates}, userSettings]) => {
          if (!userSettings) {
            return of(SA.raiseException("Failed_To_Get_Settings", null));
          }

          let settings = JSON.parse(JSON.stringify(userSettings));
          settings.templates = templates;

          return of(SA.saveSettings(settings));
        })
      ),
      { dispatch: true }
  );

  saveAccounts$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.saveAccounts),
        concatLatestFrom(() => [this.store.select(SS.userSettings)]),
        switchMap(([{accounts}, userSettings]) => {
          if (!userSettings) {
            return of(SA.raiseException("Failed_To_Add_Data", null));
          }

          let settings = JSON.parse(JSON.stringify(userSettings));
          settings.accounts = accounts;

          return of(SA.saveSettings(settings));
        })
      ),
      { dispatch: true }
  );

  saveAccount$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.saveAccount),
        exhaustMap(({account}) => {
          if (!account) {
            return of(SA.raiseException("Failed_To_Add_Data", null));
          }

          return this.settingsService.saveAccount(account).pipe(
            map(() => SA.getSettings()),
            tap(() => this.store.dispatch(SA.notifyPerson('settings.info.settingHasBeenUpdated', 'success'))),
            catchError((error) => of(SA.raiseException("Failed_To_Add_Data", error)))
          )
        })
      ),
      { dispatch: true }
  );

  deleteAccount$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.deleteAccount),
        exhaustMap(({id}) => {
          if (!id) {
            return of(SA.raiseException("Failed_To_Delete_Data", null));
          }

          return this.settingsService.deleteAccount(id).pipe(
            map(() => SA.getSettings()),
            tap(() => this.store.dispatch(SA.notifyPerson('settings.info.settingHasBeenUpdated', 'success'))),
            catchError((error) => of(SA.raiseException("Failed_To_Delete_Data", error)))
          )
        })
      ),
      { dispatch: true }
  );

  saveSettings$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.saveSettings),
        exhaustMap(({userSettings}) =>
          this.settingsService.saveUserSettings(userSettings).pipe(
            map(() => SA.getSettings()),
            tap(() => this.store.dispatch(SA.notifyPerson('settings.info.settingHasBeenUpdated', 'success'))),
            catchError((error) => of(SA.raiseException("Failed_To_Get_Settings", error)))
          )
        )
      ),
      { dispatch: true }
  );

  updateLanguage$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.updateLanguage),
        exhaustMap(({ language }) =>
          this.settingsService.updateLanguage(language).pipe(
            map((settings) => SA.getSettingsSuccess(settings)),
            catchError((error) => of(SA.raiseException("Failed_To_Get_Settings", error)))
          )
        )
      ),
      { dispatch: true }
  );

  updateLocale$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.updateLocale),
        exhaustMap(({ locale }) =>
          this.settingsService.updateLocale(locale).pipe(
            map((settings) => SA.getSettingsSuccess(settings)),
            map(({userSettings}) => SA.setLocale(userSettings.locale)),
            catchError((error) => of(SA.raiseException("Failed_To_Get_Settings", error)))
          )
        )
      ),
      { dispatch: true }
  );

  setLocale$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.setLocale),
        tap(({locale}) => {
          console.log("setLocale", locale)
          setCurrentLocale(locale);
        })
      ),
      { dispatch: false }
  );

  raiseException$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.raiseException),
        tap(({ reason, error }) => {
          console.log(error);
          if (reason == 'Failed_To_Get_Settings') {
            throw new AppError(translate('exceptions.FAILED_TO_GET_SETTINGS'), true, { severity: 'error' });
          }

          if (reason == 'Failed_To_Add_Data') {
            throw new AppError(translate('exceptions.FAILED_TO_ADD_DATA'), true, { severity: 'error' });
          }

          if (reason == 'Failed_To_Delete_Data') {
            throw new AppError(translate('exceptions.FAILED_TO_DELETE_DATA'), true, { severity: 'error' });
          }

          throw new AppError(translate('exceptions.UNSPECIFIED_ERROR'), true, {
            severity: 'error',
          });
        }),
      ),
    { dispatch: false },
  );

  notifyPerson$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(SA.notifyPerson),
        tap(({ reason, severity }) => {
          let detail = translate(reason) || 'ERROR';
          this.messageService.clear();
          this.messageService.add({
            severity: severity,
            summary: '',
            detail,
          });
        }),
      ),
    { dispatch: false },
  );
}

function setCurrentLocale(locale: string) {
  localStorage.setItem('appLocale', locale);
}
