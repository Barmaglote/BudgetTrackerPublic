import { Component, OnInit } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CreditItem } from '../../models/credit-item';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  templateUrl: './activate-credit.component.html',
  styleUrls: ['./activate-credit.component.css'],
})
export class ActivateCreditComponent implements OnInit {
  public creditItem!: CreditItem;
  public accounts!: AccountItem[];
  public isTransferMoney: boolean = false;
  public account: AccountItem | undefined;

  constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig) {}

  ngOnInit(): void {
    this.creditItem = this.config.data.creditItem;
    this.accounts = this.config.data.accounts;
  }

  activateCreditItem(creditItem: CreditItem) {
    this.ref.close({creditItem, accountItem: this.isTransferMoney ? this.account : undefined});
  }
}
