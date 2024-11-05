import { Component, OnDestroy, forwardRef, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-categories-list',
  templateUrl: './categories-list.component.html',
  styleUrls: ['./categories-list.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => CategoriesListComponent),
    multi: true,
  }],
})
export class CategoriesListComponent implements ControlValueAccessor, OnDestroy {
  ngOnDestroy(): void {
    this._valueChangesSubscription?.unsubscribe();
    this._touchesSubscription?.unsubscribe();
  }
  public items: string[] = [];
  public disabled = false;
  private touches = new Subject();
  private valueChanges = new Subject<string[]>();
  private MAX_CATEGORY_LENGHT: number = 15;
  public MAX_LIST_LENGHT: number = 15;
  private _valueChangesSubscription: Subscription | undefined;
  private _touchesSubscription: Subscription | undefined;

  writeValue(items: string[]): void {
    this.items = items;
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

  private _category: string = '';
  public get category(): string {
    return this._category;
  };

  public set category(value) {
    this._category = value;
    this.isError.set(!this.isValueCorrect(value));
  };

  public isError = signal<boolean>(false);

  addItem() {
    if (!this.category) {
      return;
    }

    if (this.items.some(x => x == this.category)) {
      return;
    }

    this.items.push(this.category);
    this.category = '';
    this.valueChanges.next(this.items);
  }

  removeCategory(category: string) {
    this.items = JSON.parse(JSON.stringify(this.items.filter(x => x != category)));
    this.valueChanges.next(this.items);
  }

  onSelect(category: string) {
    this.category = category;
  }

  private isValueCorrect(value:string): boolean {
    return value !== null && value.length <= this.MAX_CATEGORY_LENGHT && !value.includes(' ');
  }
}
