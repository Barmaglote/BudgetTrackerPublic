import { Component, Input, OnDestroy, forwardRef, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-language-selector',
  templateUrl: './language-selector.component.html',
  styleUrls: ['./language-selector.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => LanguageSelectorComponent),
    multi: true,
  }],
})
export class LanguageSelectorComponent implements ControlValueAccessor, OnDestroy {
  ngOnDestroy(): void {
    this._valueChangesSubscription?.unsubscribe();
    this._touchesSubscription?.unsubscribe();
  }

  private _valueChangesSubscription: Subscription | undefined;
  private _touchesSubscription: Subscription | undefined;
  private _initial: string = '';
  @Input() get initial() {
    return this._initial;
  };

  set initial(value: string) {
    this._initial = value;
    this.selectedLanguage = value;
  }

  public item: string = '';
  public disabled = false;
  private touches = new Subject();
  private valueChanges = new Subject<string>();

  public languageOptions = [
    { label: 'RU', value: 'ru' },
    { label: 'EN', value: 'en' },
    { label: 'DE', value: 'de' },
  ];

  public isError = signal<boolean>(false);
  writeValue(item: string): void {
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

  public selectedLanguage: string | undefined;
  public onLanguageChange() {
    if (!this.selectedLanguage) { return; }
    this.item = this.selectedLanguage;
    this.valueChanges.next(this.item);
  }
}
