import { Component, Input } from '@angular/core';
import { CreditItem } from 'src/app/modules/credits/models/credit-item';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  selector: 'app-credit-payments',
  templateUrl: './credit-payments.component.html',
  styleUrls: ['./credit-payments.component.css']
})
export class CreditPaymentsComponent{
  @Input() credits: CreditItem[] = [];
  @Input() accounts: AccountItem[] = [];
  @Input() header: string = '';
  @Input() year: number = 2024;

  getPaymentPlannedValue(item: CreditItem, month: number) {
    if (!item) { return null; }

    return item.plan?.filter(item => {
      var itemMonth = new Date(item.date).getMonth();
      var itemYear = new Date(item.date).getFullYear();
      return itemMonth+1 == month && itemYear == this.year;
    }).reduce((acc, item) => acc + item.quantity, 0);
  }

  getPaymentPaidValue(item: CreditItem, month: number) {
    if (!item) { return null; }

    return item.plan?.filter(item => {
      var itemMonth = new Date(item.date).getMonth();
      var itemYear = new Date(item.date).getFullYear();
      return itemMonth+1 == month && itemYear == this.year && item.isPaid;
    }).reduce((acc, item) => acc + item.quantity, 0);
  }

  getPaymentTotalValue(item: CreditItem) {
    if (!item) { return null; }

    return item.plan?.filter(item => {
      var itemYear = new Date(item.date).getFullYear();
      return itemYear == this.year && item.isPaid;
    }).reduce((acc, item) => acc + item.quantity, 0);
  }

  getCurrency(item: CreditItem) {
    return this.accounts.find(x => x.id == item.accountId)?.currency
  }
}
