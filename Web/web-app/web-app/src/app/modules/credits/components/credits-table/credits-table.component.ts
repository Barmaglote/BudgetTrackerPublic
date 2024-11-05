import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Table, TableLazyLoadEvent } from 'primeng/table';
import { FilterMetadata, MenuItem } from 'primeng/api';
import { Category } from 'src/app/models/category';
import { CreditItem } from '../../models/credit-item';
import { AccountItem } from 'src/app/shared/models/account-item';
import { CreditCalculator } from '../../models';
import { Payment } from '../../models/payment';
import { TransferItem } from 'src/app/shared/models/transfer-item';

@Component({
  selector: 'app-credits-table',
  templateUrl: './credits-table.component.html',
  styleUrls: ['./credits-table.component.css'],
})
export class CreditsTableComponent {
  @ViewChild('table') table!: Table;
  @Input({required: true}) items: CreditItem[] | null | undefined = [];
  @Input({required: true}) totalRecords: number = 0;
  @Input({required: true}) title!: string;
  @Input({required: true}) accounts: AccountItem[] = [];
  @Output() loadData = new EventEmitter<TableLazyLoadEvent>();
  @Output() activate = new EventEmitter<CreditItem>();
  @Output() info = new EventEmitter<CreditItem>();
  @Output() update = new EventEmitter<CreditItem>();
  @Output() delete = new EventEmitter<CreditItem>();
  @Output() transfer = new EventEmitter<{transferItem: TransferItem | undefined, creditId: string | undefined}>();

  public Math = Math;

  public itemDialog: boolean = false;
  public item!: CreditItem;
  public category: Category | undefined;
  public submitted: boolean = false;
  public filterMetadata = { value: '', matchMode: 'contains', operator: 'comment' } as FilterMetadata;
  public comment: string | undefined = undefined;
  public rangeDates: Date[] | undefined;
  public isGrouped: boolean = false;
  public onIconClass: string ="pi pi-minus-circle";
  public offIconClass: string ="pi pi-plus-circle";

  private currentItem!: CreditItem;

  selectItem(item: CreditItem) {
    this.currentItem = item;
  }

  public menuItems: MenuItem[] = [
    {
      label: 'Info',
      icon: 'pi pi-info',
      command: () => { this.infoItem(this.currentItem) }
    },
    {
      label: 'Activate',
      icon: 'pi pi-play',
      command: () => { this.activateItem(this.currentItem) }
    },
    {
      label: 'Delete',
      icon: 'pi pi-trash',
      command: () => { this.deleteItem(this.currentItem) }
    },
  ]

  constructor() { }

  onLoadData(event: TableLazyLoadEvent) {
    this.loadData.emit(event);
  }

  infoItem(item: CreditItem) {
    this.info.emit(item);
  }

  activateItem(item: CreditItem) {
    this.activate.emit(item);
  }

  deleteItem(item: CreditItem) {
    this.delete.emit(item);
  }

  hideDialog() {
    this.itemDialog = false;
    this.submitted = false;
  }

  public getTotalRepayment(loanAmount: number, annualInterestRate: number, mandatoryPayment: number, lumpSumPayment: boolean, loanTermMonths: number, startDate: Date) {
    const calculator = new CreditCalculator(
      loanAmount,
      annualInterestRate,
      mandatoryPayment,
      lumpSumPayment,
      loanTermMonths,
      startDate
    );

    return calculator.getTotalRepayment();
  }

  public getTotalInterestPaid(loanAmount: number, annualInterestRate: number, mandatoryPayment: number, lumpSumPayment: boolean, loanTermMonths: number, startDate: Date) {
    const calculator = new CreditCalculator(
      loanAmount,
      annualInterestRate,
      mandatoryPayment,
      lumpSumPayment,
      loanTermMonths,
      startDate
    );

    return calculator.getTotalInterestPaid();
  }

  public getPaymentSchedule(loanAmount: number, annualInterestRate: number, mandatoryPayment: number, lumpSumPayment: boolean, loanTermMonths: number, startDate: Date) {
    const calculator = new CreditCalculator(
      loanAmount,
      annualInterestRate,
      mandatoryPayment,
      lumpSumPayment,
      loanTermMonths,
      startDate
    );

    return calculator.getPaymentSchedule();
  }

  public getAccount(accountId: string) {
    if (!this.accounts || this.accounts.length === 0) {
      return null;
    }

    return this.accounts.find(x => x.id === accountId);
  }

  onPlanChanged(plan: Payment[] | undefined, creditItem: CreditItem) {
    this.update.emit({...creditItem, plan} as CreditItem);
  }

  onTransfer(transferItem: TransferItem | undefined, creditId: string | undefined) {
    this.transfer.emit({transferItem, creditId});
  }
}
