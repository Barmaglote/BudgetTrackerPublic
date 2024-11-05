import { APP_INITIALIZER, ErrorHandler, LOCALE_ID, NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule as CustomSharedModule } from './shared/shared.module';
import { FormsModule } from '@angular/forms';
import { provideStore, StoreModule } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { EffectsModule, provideEffects } from '@ngrx/effects';
import { HttpClientModule } from '@angular/common/http';
import { HTTP_INTERCEPTORS_PROVIDERS } from './core/interceptors'
import { GlobalErrorHandler } from './core/error-handler';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TranslocoModule, provideTransloco } from '@ngneat/transloco';
import { LanguageSettingsService, TranslocoHttpLoader, translocoLoader } from './core/services';
import { environment } from 'src/environments/environment';
import { ToastModule } from 'primeng/toast';
import { SOCIAL_LOGIN_PROVIDER } from './core/providers';
import { MenubarModule } from 'primeng/menubar';
import { SETTINGS_PROVIDERS } from './modules/settings/store';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

export function initializeApp(languageSettingsService: LanguageSettingsService) {
  return () => languageSettingsService.initializeApp();
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    MenubarModule,
    ConfirmDialogModule,
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    CustomSharedModule,
    TranslocoModule,
    FormsModule,
    ToastModule,
    StoreModule.forRoot(
      {},
      {
        runtimeChecks: {
          strictActionImmutability: true,
          strictActionSerializability: false,
          strictStateImmutability: true,
          strictStateSerializability: false,
        },
      },
    ),
    EffectsModule.forRoot([]),
  ],
  providers: [
    provideTransloco({
      config: {
          availableLangs: ['en', 'de', 'ru'],
          defaultLang: 'en',
          reRenderOnLangChange: true,
          prodMode: environment.production,
      },
      loader: TranslocoHttpLoader
    }),
    translocoLoader,
    SOCIAL_LOGIN_PROVIDER,
    provideStoreDevtools({ maxAge: 25 }),
    provideStore({}),
    provideEffects(),
    MessageService,
    ...HTTP_INTERCEPTORS_PROVIDERS,
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    {
      provide: LOCALE_ID,
      deps: [LanguageSettingsService],
      useFactory: LocalIDFactory,
      useValue: 'en'
    },
    ConfirmationService,
    LanguageSettingsService,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      multi: true,
      deps: [LanguageSettingsService]
    },
    ...SETTINGS_PROVIDERS
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(languageSettingsService: LanguageSettingsService ) {
    const langToSet = languageSettingsService.getLocale();

    languageSettingsService.registerCulture(langToSet);
  }
}

export function LocalIDFactory(languageSettingsService: LanguageSettingsService) {
  return languageSettingsService.getLocale();
}
