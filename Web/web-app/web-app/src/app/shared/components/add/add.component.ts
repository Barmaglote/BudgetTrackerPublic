import { Component, EventEmitter, Inject, Input, LOCALE_ID, Output } from '@angular/core';
import { Item } from '../../../modules/item/models';
import { Category } from 'src/app/models/category';
import { TemplateItem } from 'src/app/shared/models/template-item';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { categoryValidator } from '../../../modules/item/models/category-validator';
import { AccountItem } from 'src/app/shared/models/account-item';
import { BehaviorSubject } from 'rxjs';
import { PlannedItem } from '../../models/planned-item';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.css']
})
export class AddComponent {
  @Input({required : true}) categories: Category[] | undefined;
  @Input({required : true}) templates: TemplateItem[] | undefined;
  @Input({required: true}) accounts: AccountItem[] = [];

  private _plannedItem: PlannedItem | undefined;
  @Input({required: false}) get plannedItem(): PlannedItem | undefined {
    return this._plannedItem;
  };

  set plannedItem(value) {
    this._plannedItem = value;

    if (this.plannedItem) {
      this.newItemForm.setValue({quantity: this.plannedItem.quantity, comment: this.plannedItem.comment, account: null, category: null, date: new Date(this.plannedItem.date), isRegular: false});
    }
  }

  public template: TemplateItem | undefined;
  @Output() add = new EventEmitter<Item>();

  public selectedAccount$: BehaviorSubject<AccountItem | null> = new BehaviorSubject<AccountItem | null>(null);

  public newItemForm: FormGroup = this.formBuilder.group({
    account: [null, [Validators.required]],
    category: [null, [Validators.required, categoryValidator()]],
    date: [null, [Validators.required]],
    comment: [''],
    quantity: [0, [Validators.required, Validators.min(1)]],
    isRegular: [false]
  });

  constructor(private formBuilder: FormBuilder, @Inject(LOCALE_ID) public locale: string) {}

  onSubmit(){
    let {date, comment, quantity, isRegular, category, account} = this.newItemForm.value;

    if (this.newItemForm.invalid) {
      this.newItemForm.markAllAsTouched();
      return;
    }

    this.add.emit({date, comment, quantity, isRegular, category: category?.category, accountId: account.id} as Item);
  }

  onChangeTemplate(){

    let category = {};

    if (this.template?.category) {
      const filtered = this.categories?.filter(x => x.category === this.template?.category);
      if (filtered && filtered.length > 0) {
        category = filtered[0];
      }
    }

    let account = null

    if (this.template?.accountId) {
      account = this.accounts.find(x => x.id == this.template?.accountId);
      this.selectedAccount$.next(account || null);
    }

    this.newItemForm.setValue({
      category,
      comment: this.template?.comment || '',
      quantity: this.template?.quantity || 0,
      isRegular: this.template?.isRegular || false,
      date: null,
      account: account || null,
    });
  }

  onAccountChange(event: any){
    this.selectedAccount$.next(event.value);
  }
}
