import { Injectable, OnDestroy } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeDe from '../../../assets/locales/de';
import localeEn from '../../../assets/locales/en';
import localeRu from '@angular/common/locales/ru';
import * as featureStore from '../../modules/settings/store';
import { Observable, Subscription } from 'rxjs';
import { Store } from '@ngrx/store';
import { UserSettings } from 'src/app/models/user-settings';
import { TranslocoService } from '@ngneat/transloco';

@Injectable({ providedIn: 'root' })
export class LanguageSettingsService implements OnDestroy {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureStore.SettingsSelectors.userSettings);
  private _userSettingsSubscription: Subscription | undefined;

  allowedLanguage = ['en', 'de', 'ru'];
  lang = '';

  constructor(
    private store: Store,
    private translocoService: TranslocoService
  ) {}
  ngOnDestroy(): void {
    if (this._userSettingsSubscription) {
      this._userSettingsSubscription.unsubscribe();
    }
  }

  initializeApp(): Promise<any> {
    this.lang = this.getDefaultLocale();
    const naviLang = this.getLanguageMap(navigator.language);
    if (naviLang && this.allowedLanguage.indexOf(naviLang) >= 0) {
      this.lang = naviLang;
    }

    this._userSettingsSubscription = this.userSettings$.subscribe((settings) => {
      if (settings?.language) {
        this.translocoService.setActiveLang(settings?.language || 'en');
      }
    });
    return Promise.resolve();
  }

  public registerCulture(lang: string) {
    if (!lang) {
      return;
    }

    switch (lang) {
      case 'de': {
        registerLocaleData(localeDe, 'de');
        break;
      }
      case 'de-DE': {
        registerLocaleData(localeDe, 'de');
        break;
      }
      case 'en': {
        registerLocaleData(localeEn, 'en');
        break;
      }
      case 'en-US': {
        registerLocaleData(localeEn, 'en');
        break;
      }
      case 'ru': {
        registerLocaleData(localeRu, 'ru');
        break;
      }
      case 'ru-RU': {
        registerLocaleData(localeRu, 'ru');
        break;
      }
      default: {
        registerLocaleData(localeEn, 'en');
        break;
      }
    }

    localStorage.setItem('appLocale', lang);
  }

  public getLanguageMap(lang: string) {
    if (!lang) {
      return;
    }

    if (lang.indexOf('de-') >= 0) {
      return 'de';
    }

    if (lang.indexOf('en-') >= 0) {
      return 'en';
    }

    if (lang.indexOf('ru-') >= 0) {
      return 'ru';
    }

    return lang;
  }

  public getLocale(): string {
    return localStorage.getItem('appLocale') || 'en-US';
  }

  public getDefaultLocale(): string {
    return 'en';
  }
}
