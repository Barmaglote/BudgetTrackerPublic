import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { Payment } from '../../models/payment';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PaymentItemChangerComponent } from '../../dialogs';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { MakePaymentComponent } from 'src/app/shared/dialogs';
import { translate } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-payment-plan',
  templateUrl: './payment-plan.component.html',
  styleUrls: ['./payment-plan.component.css'],
  providers: [DialogService]
})
export class PaymentPlanComponent implements OnDestroy {
  private _paymentPlan: Payment[] | undefined;
  public repayment: number = 0;
  public overpayment: number = 0;
  @Input() columnWidthClass: string = 'w-3';
  @Input() loan : number = 0;
  @Input() accountId! : string;
  @Input() accounts : AccountItem[] | undefined;
  @Input() isReadOnly : boolean = false;
  @Input() get paymentPlan(): Payment[] | undefined {
    return this._paymentPlan;
  }

  @Output() changed = new EventEmitter<Payment[] | undefined>();
  @Output() transfer = new EventEmitter<TransferItem | undefined>();

  set paymentPlan(value) {
    this._paymentPlan = value;
    this.recalculate();
  }
  @Input() currency: string | undefined;
  ref: DynamicDialogRef | undefined;

  constructor(public dialogService: DialogService) {}
  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([this.ref]);
    garbageCollector.collect();
  }

  onClick(payment: Payment) {
    if (this.isReadOnly) {
      return;
    }

    let copy = JSON.parse(JSON.stringify(payment));
    copy.date = new Date(payment.date);

    this.ref = this.dialogService.open(PaymentItemChangerComponent, {
      header: translate('credits.modify_payment'),
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: false,
      draggable: true,
      data: { payment: copy }
    });

    this.ref.onClose.subscribe((payment: Payment) => {
      if (payment) {
        const index = this.paymentPlan?.findIndex(x => x.month === payment.month);
        if (index && index > -1 && this.paymentPlan) {
          let plan = JSON.parse(JSON.stringify(this.paymentPlan));
          plan[index] = payment;
          this.paymentPlan = plan;
          this.recalculate();
          this.changed.emit(this.paymentPlan);
        }
      }
    });
  }

  recalculate() {
    this.repayment = this._paymentPlan?.reduce((acc,v) => acc + v.quantity, 0) || 0;
    this.overpayment = (this._paymentPlan?.reduce((acc,v) => acc + v.quantity, 0) || 0) - this.loan;
  }

  makePayment(payment: Payment) {
    this.ref = this.dialogService.open(MakePaymentComponent, {
      header: 'Make payment',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: false,
      draggable: true,
      width: '30rem',
      data: { toAccountId: this.accountId, accounts: this.accounts, quantity: payment.quantity, date: payment.date }
    });

    this.ref.onClose.subscribe((transferItem: TransferItem) => {
      this.transfer.emit(transferItem);
    });
  }
}
