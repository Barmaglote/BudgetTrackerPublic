import { Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { UserSettings } from 'src/app/models/user-settings';
import { Observable, Subscription } from 'rxjs';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-language',
  templateUrl: './language.component.html',
  styleUrls: ['./language.component.css']
})
export class LanguageComponent implements OnInit, OnDestroy {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureStore.SettingsSelectors.userSettings);
  public initialLanguage: string = '';
  public initialLocale: string = '';
  private _userSettingsSubscription: Subscription | undefined;

  public languageForm: FormGroup = this.formBuilder.group({
    language: [],
    locale: [],
  });

  constructor(private formBuilder: FormBuilder, private store: Store) { }
  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([this.userSettings$]);
    garbageCollector.collect();
    this._userSettingsSubscription?.unsubscribe();
  }

  ngOnInit() {
    this._userSettingsSubscription = this.userSettings$.subscribe(
      x =>
        {
          let {language} = this.languageForm.value;
          if (x?.language && !language) {
            this.initialLanguage = x?.language || '';
          }

          let {locale} = this.languageForm.value;
          if (x?.locale && !locale) {
            this.initialLocale = x?.locale || '';
          }
        }
    );
  }

  onSave(){
    if (this.languageForm.invalid) {
      this.languageForm.markAllAsTouched();
      return;
    }

    let {language} = this.languageForm.value;
    let {locale} = this.languageForm.value;
    if (language) {
      this.store.dispatch(featureStore.SettingsActions.updateLanguage(language));
    }

    if (locale) {
      this.store.dispatch(featureStore.SettingsActions.updateLocale(locale));
    }
  }

}
