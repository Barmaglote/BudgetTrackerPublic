import { Component, Inject, Input, LOCALE_ID, OnDestroy, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, Subscription } from 'rxjs';
import { AccountType } from 'src/app/shared/enums';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  selector: 'app-accounts-editor',
  templateUrl: './accounts-editor.component.html',
  styleUrls: ['./accounts-editor.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => AccountsEditorComponent),
    multi: true,
  }],
})
export class AccountsEditorComponent implements ControlValueAccessor, OnDestroy {
  public item: AccountItem = {
    id: undefined,
    title: '',
    initial: 0,
    quantity: 0,
    limit: 0,
    comment:  '',
    accountType: undefined,
    currency: '',
    isDeleted: false,
    goal: 0
  };

  public disabled = false;
  private touches = new Subject();
  private valueChanges = new Subject<AccountItem>();
  public AccountType = AccountType;
  private _valueChangesSubscription: Subscription | undefined;
  private _touchesSubscription: Subscription | undefined;

  constructor(@Inject(LOCALE_ID) public locale: string) {}
  ngOnDestroy(): void {
    this._valueChangesSubscription?.unsubscribe();
    this._touchesSubscription?.unsubscribe();
  }

  writeValue(item: AccountItem): void {
    this.item = item;
  }
  registerOnChange(fn: any): void {
    this._valueChangesSubscription = this.valueChanges.subscribe(fn);
  }
  registerOnTouched(fn: any): void {
    this._touchesSubscription = this.touches.subscribe(fn);
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onCurrencyChange(currency: string){
    this.item.currency = currency;
    this.valueChanges.next(this.item);
  }

  onAccountTypeChange(accountType: AccountType){
    this.item.accountType = accountType;
    this.valueChanges.next(this.item);
  }

  onTitleChange(){
    this.valueChanges.next(this.item);
  }
}
