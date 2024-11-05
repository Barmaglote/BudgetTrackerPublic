import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { Observable, Subscription } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css'],
})
export class CategoriesComponent implements OnInit, OnDestroy {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureStore.SettingsSelectors.userSettings);

  public categoriesForm: FormGroup = this.formBuilder.group({
    income: [[]],
    expenses: [[]],
    credits: [[]],
  });

  private _userSettingsSubscription: Subscription | undefined;

  constructor(private formBuilder: FormBuilder, private store: Store) {}

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([this.userSettings$]);
    garbageCollector.collect();
    this._userSettingsSubscription?.unsubscribe();
  }

  ngOnInit(): void {
    this._userSettingsSubscription = this.userSettings$.subscribe((userSettings: UserSettings | undefined) => {
      this.categoriesForm.setValue({
        income: userSettings?.categories ? JSON.parse(JSON.stringify(userSettings.categories['income'] || [])) : [],
        expenses: userSettings?.categories ? JSON.parse(JSON.stringify(userSettings.categories['expenses'] || [])) : [],
        credits: userSettings?.categories ? JSON.parse(JSON.stringify(userSettings.categories['credits'] || [])) : []
      });
    });
  }

  onSave() {
    if (this.categoriesForm.invalid) {
      this.categoriesForm.markAllAsTouched();
      return;
    }

    let {income, expenses, credits} = this.categoriesForm.value;
    this.store.dispatch(featureStore.SettingsActions.saveCategories({income, expenses, credits}));
  }
}
