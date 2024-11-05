import { Component, EventEmitter, Inject, Input, LOCALE_ID, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TransferItem } from 'src/app/shared/models/transfer-item';

@Component({
  templateUrl: './make-payment.component.html',
  styleUrls: ['./make-payment.component.css']
})
export class MakePaymentComponent implements OnInit {
  public accounts: AccountItem[] | undefined;
  public to: AccountItem | undefined;
  public toQuantity!: number;
  private date!: Date;
  @Output() add = new EventEmitter<TransferItem>();

  public newItemForm: FormGroup = this.formBuilder.group({
    fromAccount: [null, [Validators.required]],
    toAccount: [null, [Validators.required]],
    fromQuantity: [0, [Validators.required, Validators.min(1)]],
    toQuantity: [0, [Validators.required, Validators.min(1)]],
  });

  constructor(private formBuilder: FormBuilder, public ref: DynamicDialogRef, public config: DynamicDialogConfig, @Inject(LOCALE_ID) public locale: string) {}

  ngOnInit(): void {
    var { toAccountId, accounts, quantity, date } = this.config.data;
    this.accounts = accounts;
    this.to = (accounts as AccountItem[] | undefined)?.find(x => x.id == toAccountId) || undefined;
    this.toQuantity = quantity;
    this.date = date;
    this.newItemForm.setValue({toQuantity: quantity, toAccount: this.to, fromAccount: null, fromQuantity: 0 });
  }

  onSubmit(){
    let {fromAccount, toAccount, fromQuantity, toQuantity} = this.newItemForm.value;

    if (this.newItemForm.invalid) {
      this.newItemForm.markAllAsTouched();
      return;
    }

    this.ref.close({fromAccount, toAccount, fromQuantity, toQuantity, date: this.date } as TransferItem);
  }
}
