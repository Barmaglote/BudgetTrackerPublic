import { Component, Input, OnDestroy, forwardRef, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-locale-selector',
  templateUrl: './locale-selector.component.html',
  styleUrls: ['./locale-selector.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => LocaleSelectorComponent),
    multi: true,
  }],
})
export class LocaleSelectorComponent implements ControlValueAccessor, OnDestroy {
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
    this.selectedLocale = value;
  }

  public item: string = '';
  public disabled = false;
  private touches = new Subject();
  private valueChanges = new Subject<string>();

  public localOptions = [
    { code: 'ru-RU', label: 'Russian (Russia)' },
    { code: 'en-US', label: 'English (United States)' },
    { code: 'de-DE', label: 'German (Germany)' }
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

  public selectedLocale: string | undefined;
  public onLanguageChange() {
    if (!this.selectedLocale) { return; }
    this.item = this.selectedLocale;
    this.valueChanges.next(this.item);
  }
}
