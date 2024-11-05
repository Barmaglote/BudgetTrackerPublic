import { Component, EventEmitter, OnDestroy, Output, forwardRef } from '@angular/core';
import { AccountType } from '../../enums';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, Subscription } from 'rxjs';
import { ConfirmationService } from 'primeng/api';
import { translate } from '@ngneat/transloco';

@Component({
  selector: 'app-account-type-selector',
  templateUrl: './account-type-selector.component.html',
  styleUrls: ['./account-type-selector.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => AccountTypeSelectorComponent),
    multi: true,
  }],
})
export class AccountTypeSelectorComponent implements ControlValueAccessor, OnDestroy {
  @Output() accountTypeChange = new EventEmitter<AccountType>();
  constructor(private confirmationService: ConfirmationService) {}
  ngOnDestroy(): void {
    this._valueChangesSubscription?.unsubscribe();
    this._touchesSubscription?.unsubscribe();
  }
  private _valueChangesSubscription: Subscription | undefined;
  private _touchesSubscription: Subscription | undefined;

  private last: AccountType | undefined;
  selectedAccountType: AccountType | undefined;
  private valueChanges = new Subject<AccountType>();

  accountTypeOptions = [
    { label: 'Cash', value: AccountType.cash },
    { label: 'Debit Card', value: AccountType.debitCard },
    { label: 'Credit Card', value: AccountType.creditCard },
    { label: 'Deposit', value: AccountType.deposit },
    { label: 'Credit', value: AccountType.credit },
  ];

  onAccountTypeChange() {
    if (this.last) {
      this.confirmationService.confirm({
        message: translate('shared.are-you-sure-you-want-to-change-the-type'),
        header: translate('shared.confirmation'),
        icon: 'pi pi-info-circle',
        accept: () => {
          this.accountTypeChange.emit(this.selectedAccountType);
        },
        reject: () => {
          this.selectedAccountType = this.last;
        }
      });
    } else {
      this.accountTypeChange.emit(this.selectedAccountType);
    }
  }

  public disabled = false;
  private touches = new Subject();

  writeValue(selectedAccountType: AccountType): void {
    this.last = this.selectedAccountType;
    this.selectedAccountType = selectedAccountType;
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
}
