import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountItem } from 'src/app/shared/models/account-item';
import { TemplateItem } from 'src/app/shared/models/template-item';

@Component({
  selector: 'app-regualar-payments',
  templateUrl: './regualar-payments.component.html',
  styleUrls: ['./regualar-payments.component.css']
})
export class RegualarPaymentsComponent {
  @Input() templatesIncome?: TemplateItem[];
  @Input() templatesExpenses?: TemplateItem[];
  @Input() accounts: AccountItem[] | undefined;
  @Output() payment = new EventEmitter<{templateItem: TemplateItem,  area: string}>();

  getAccount(accountId: string) {
    if (!accountId || !this.accounts) {
      return null;
    }

    return this.accounts.find(x => x.id == accountId);
  }

  makePayment(templateItem: TemplateItem, area: string) {
    this.payment.emit({templateItem, area});
  }
}
