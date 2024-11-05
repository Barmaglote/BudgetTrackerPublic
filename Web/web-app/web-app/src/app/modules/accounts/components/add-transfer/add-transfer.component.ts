import { Component, EventEmitter, Inject, Input, LOCALE_ID, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TransferItem } from 'src/app/shared/models/transfer-item';

@Component({
  selector: 'app-add-transfer',
  templateUrl: './add-transfer.component.html',
  styleUrls: ['./add-transfer.component.css']
})
export class AddTransferComponent {
  @Input({required : true}) accounts: AccountItem[] | undefined;
  @Input({required : false}) form: AccountItem | undefined;
  @Input({required : false}) to: AccountItem | undefined;
  @Output() add = new EventEmitter<TransferItem>();

  public newItemForm: FormGroup = this.formBuilder.group({
    fromAccount: [null, [Validators.required]],
    toAccount: [null, [Validators.required]],
    fromQuantity: [0, [Validators.required, Validators.min(1)]],
    toQuantity: [0, [Validators.required, Validators.min(1)]],
  });

  constructor(private formBuilder: FormBuilder, @Inject(LOCALE_ID) public locale: string) {}

  onSubmit(){
    let {fromAccount, toAccount, fromQuantity, toQuantity} = this.newItemForm.value;

    if (this.newItemForm.invalid) {
      this.newItemForm.markAllAsTouched();
      return;
    }

    this.add.emit({fromAccount, toAccount, fromQuantity, toQuantity } as TransferItem);
  }
}
