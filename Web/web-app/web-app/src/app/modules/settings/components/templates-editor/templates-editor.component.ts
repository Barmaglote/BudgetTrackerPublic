import { Component, Inject, Input, LOCALE_ID, OnDestroy, OnInit, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, Subscription } from 'rxjs';
import { Category } from 'src/app/models/category';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TemplateItem } from 'src/app/shared/models/template-item';

@Component({
  selector: 'app-templates-editor',
  templateUrl: './templates-editor.component.html',
  styleUrls: ['./templates-editor.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => TemplatesEditorComponent),
    multi: true,
  }],
})
export class TemplatesEditorComponent implements ControlValueAccessor, OnDestroy {
  @Input({required: true}) categories: Category[] = [];
  @Input({required: true}) accounts: AccountItem[] = [];
  public item: TemplateItem = {
    title: '',
    category: '',
    comment: '',
    quantity: 0,
    isRegular: false,
    accountId: ''
  };

  public category: Category | undefined;
  public account: AccountItem | undefined;
  public disabled = false;
  private touches = new Subject();
  private valueChanges = new Subject<TemplateItem>();
  private _valueChangesSubscription: Subscription | undefined;
  private _touchesSubscription: Subscription | undefined;

  constructor(@Inject(LOCALE_ID) public locale: string) {}
  ngOnDestroy(): void {
    this._valueChangesSubscription?.unsubscribe();
    this._touchesSubscription?.unsubscribe();
  }

  writeValue(item: TemplateItem): void {
    this.item = item;

    if (item.accountId && this.accounts) {
      const account = this.accounts.find(x => x.id === item.accountId);
      this.item.accountId = account?.id || '';
      this.account = account;
    }
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

  onTitleChange(){
    this.valueChanges.next(this.item);
  }

  onAccountChange(){
    this.item.accountId = this.account?.id || '';
    this.valueChanges.next(this.item);
  }
}
