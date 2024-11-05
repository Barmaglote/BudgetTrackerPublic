import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { AccountItem } from 'src/app/shared/models/account-item';
import { PaymentInfo } from '../../../modules/credits/models/payment-info';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { MakePaymentComponent } from '../../dialogs';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-next-payments',
  templateUrl: './next-payments.component.html',
  styleUrls: ['./next-payments.component.css']
})
export class NextPaymentsComponent implements OnDestroy{
  @Input() accounts: AccountItem[] | undefined;
  @Input() payments: PaymentInfo[] | undefined;
  ref: DynamicDialogRef | undefined;
  @Output() transfer = new EventEmitter<{transferItem: TransferItem | undefined, creditId: string | undefined}>();
  private _transferSubscription: Subscription | undefined;

  constructor(public dialogService: DialogService) { }
  ngOnDestroy(): void {
    if (this._transferSubscription) {
      this._transferSubscription.unsubscribe();
    }
  }

  getAccount(accountId: string) {
    if (!accountId || !this.accounts) {
      return null;
    }

    return this.accounts.find(x => x.id == accountId);
  }

  makePayment(paymentInfo: PaymentInfo) {
    this.ref = this.dialogService.open(MakePaymentComponent, {
      header: 'Make payment',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: false,
      draggable: true,
      width: '30rem',
      data: { toAccountId: paymentInfo.accountId, accounts: this.accounts, quantity: paymentInfo.payment.quantity, date: paymentInfo.payment.date }
    });

    this._transferSubscription = this.ref.onClose.subscribe((transferItem: TransferItem) => {
      this.transfer.emit({transferItem, creditId: paymentInfo.creditId});
    });
  }
}
