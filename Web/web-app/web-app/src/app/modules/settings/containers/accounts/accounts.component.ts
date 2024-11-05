import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable, Subscription, map } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import * as featureStore from '../../store';
import { GarbageCollector } from 'src/app/models';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { AccountItem } from 'src/app/shared/models/account-item';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.css']
})
export class AccountsComponent implements OnDestroy, OnInit {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureStore.SettingsSelectors.userSettings);
  public selected: AccountItem | undefined;
  public max: number = 10;
  public currentItemInForm: AccountItem | undefined;
  public account: string | undefined | null;
  private _accountsSubscription: Subscription | undefined;
  private _accountsFormSubscription: Subscription | undefined;

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => userSettings?.accounts || [])
  )

  public accountsForm: FormGroup = this.formBuilder.group({
    item: [{}]
  });

  constructor(
    private activatedRoute: ActivatedRoute,
    private store: Store,
    private formBuilder: FormBuilder,
    private confirmationService: ConfirmationService) {
  }

  ngOnInit(): void {
    this._accountsFormSubscription = this.accountsForm.valueChanges.subscribe((value) => {
      this.currentItemInForm = value.item;
    });

    this.account = this.activatedRoute.snapshot.queryParamMap.get('account');
    if (this.account) {
      this._accountsSubscription = this.accounts$.subscribe(accounts => {
        if (accounts) {
          this.selected = accounts.find(x => x.id === this.account);
          if (this.selected) {
            this.accountsForm.setValue({ item: JSON.parse(JSON.stringify(this.selected)) });
          } else {
            this.accountsForm.setValue({ item: undefined });
          }
        }
      });
    }
  }

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.userSettings$,
      this.accounts$
    ]);
    garbageCollector.collect();
    this._accountsFormSubscription?.unsubscribe();
    this._accountsSubscription?.unsubscribe();
  }

  async onSave(){
    if (this.accountsForm.invalid) {
      this.accountsForm.markAllAsTouched();
      return;
    }

    let { item } = this.accountsForm.value;
    this.store.dispatch(featureStore.SettingsActions.saveAccount(item));
    this.onNew();
  }

  onNew(){
    this.accountsForm.setValue({ item: {initial: 0, currency: null } });
  }

  async onDelete(){
    let { item } = this.accountsForm.value;

    if (!item?.title || !item?.id) {
      this.onNew();
      return;
    }

    this.confirmationService.confirm({
      message: 'Are you sure you want to delete Account ' + item.title + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.store.dispatch(featureStore.SettingsActions.deleteAccount(item.id));
        this.onNew();
      }
    });
  }

  onSelect() {
    if (this.selected) {
      this.accountsForm.setValue({ item: JSON.parse(JSON.stringify(this.selected)) });
    } else {
      this.accountsForm.setValue({ item: undefined });
    }
  }
}
