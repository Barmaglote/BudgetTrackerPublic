import { Component, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Payment } from '../../models/payment';

@Component({
  templateUrl: './payment-item-changer.component.html',
  styleUrls: ['./payment-item-changer.component.css'],
})
export class PaymentItemChangerComponent implements OnInit {
  payment!: Payment;

  constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig) {}

  ngOnInit(): void {
    this.payment = this.config.data.payment;
  }

  updatePaymentItem(payment: Payment) {
    this.ref.close(payment);
  }
}
