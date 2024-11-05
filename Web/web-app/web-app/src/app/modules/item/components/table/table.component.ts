import { Component, EventEmitter, Inject, Input, LOCALE_ID, Output, ViewChild } from '@angular/core';
import { Item } from '../../models';
import { Table, TableLazyLoadEvent } from 'primeng/table';
import { ConfirmationService, FilterMetadata, MenuItem } from 'primeng/api';
import { Category } from 'src/app/models/category';
import * as FileSaver from 'file-saver';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css'],
})
export class TableComponent {
  @ViewChild('table') table!: Table;
  @Input({required: true}) items: Item[] | null | undefined = [];
  @Input({required: true}) categories: Category[] = [];
  @Input({required: true}) totalRecords: number = 0;
  @Input({required: true}) title!: string;
  @Input({required: true}) accounts: AccountItem[] = [];
  @Output() loadData = new EventEmitter<TableLazyLoadEvent>();
  @Output() delete = new EventEmitter<Item>();
  @Output() update = new EventEmitter<Item>();
  @Output() info = new EventEmitter<Item>();
  @Output() rollbackTransaction = new EventEmitter<Item>();
  @Output() infoTransaction = new EventEmitter<Item>();
  public Math = Math;

  public itemDialog: boolean = false;
  public item!: Item;
  public category: Category | undefined;
  public submitted: boolean = false;
  public filterMetadata = { value: '', matchMode: 'contains', operator: 'comment' } as FilterMetadata;
  public comment: string | undefined = undefined;
  public rangeDates: Date[] | undefined;
  public isGrouped: boolean = false;
  public onIconClass: string ="pi pi-minus-circle";
  public offIconClass: string ="pi pi-plus-circle";

  private currentItem!: Item;

  selectItem(item: Item) {
    this.currentItem = item;
  }

  public getAccount(accountId: string) {
    if (!this.accounts || this.accounts.length === 0) {
      return null;
    }

    return this.accounts.find(x => x.id === accountId);
  }

  menuTransactionItems: MenuItem[] = [
    {
      label: 'Info',
      icon: 'pi pi-info',
      command: () => { this.infoTransaction.emit(this.currentItem) }
    },
    {
      label: 'Rollback',
      icon: 'pi pi-trash',
      command: () => { this.deleteTransaction(this.currentItem) }
    },
  ]

  public menuItems: MenuItem[] = [
    {
      label: 'Info',
      icon: 'pi pi-info',
      command: () => { this.infoItem(this.currentItem) }
    },
    {
      label: 'Edit',
      icon: 'pi pi-file-edit',
      command: () => { this.editItem(this.currentItem) }
    },
    {
      label: 'Delete',
      icon: 'pi pi-trash',
      command: () => { this.deleteItem(this.currentItem) }
    },
  ]

  public getCategory(category: string | undefined) {
    if (!category) return null;
    const filtered = this.categories.filter(x => x.category === category);
    return filtered ? filtered[0] : null;
  }

  constructor(private confirmationService: ConfirmationService, @Inject(LOCALE_ID) public locale: string) { }

  onLoadData(event: TableLazyLoadEvent) {
    this.loadData.emit(event);
  }

  infoItem(item: Item) {
    this.info.emit(item);
  }

  editItem(item: Item) {
    this.item = JSON.parse(JSON.stringify(item));

    const filtered = this.categories.filter(x => x.category === item.category);
    this.category = filtered && filtered.length > 0 ? filtered[0] : undefined;
    this.itemDialog = true;
  }

  deleteItem(item: Item) {
    this.confirmationService.confirm({
        message: 'Are you sure you want to delete record $' + item.quantity + '?',
        header: 'Confirm',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.delete.emit(item);
        }
    });
  }

  deleteTransaction(item: Item){
    this.confirmationService.confirm({
      message: 'Are you sure you want to rollback transaction?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.rollbackTransaction.emit(item);
      }
  });
  }

  hideDialog() {
    this.itemDialog = false;
    this.submitted = false;
  }

  saveItem() {
    this.itemDialog = false;
    this.submitted = true;
    this.item.category = this.category?.category || '';
    this.update.emit(this.item);
  }

  calculateSubTotal(month: string) {
    if (!this.items || this.items.length == 0) {
      return null;
    }

    let monthlyData = this.items.filter(item => item.month === month);

    if (!monthlyData || monthlyData.length == 0) {
      return null;
    }

    let aggregatedData: { [key: string]: number } = {};
    monthlyData.forEach((item: Item) => {

      const currency = this.getAccount(item.accountId)?.currency;
      if (aggregatedData[currency || '']) {
        aggregatedData[currency || ''] += Math.floor(item.quantity*100)/100;
      } else {
        aggregatedData[currency || ''] = Math.floor(item.quantity*100)/100;
      }

    });

    const result = Object.keys(aggregatedData).map(x => `${x}: ${Math.floor(aggregatedData[x]*100)/100}`).join(', ');
    return result;
  }

  aggregateData(currency: string | undefined) {
    if (!currency) {
      return null;
    }

    if (!this.items || this.items.length == 0) {
      return null;
    }

    let aggregatedData: { [key: string]: number } = {};
    this.items.forEach((item: Item) => {
      const currency = this.getAccount(item.accountId)?.currency;
      if (aggregatedData[currency || '']) {
        aggregatedData[currency || ''] += Math.floor(item.quantity*100)/100;
      } else {
        aggregatedData[currency || ''] = Math.floor(item.quantity*100)/100;
      }
    });

    return Math.floor(aggregatedData[currency]*100)/100;
  }

  exportExcel() {
    if (!this.items) { return; }
    import('xlsx').then((xlsx) => {
      const worksheet = xlsx.utils.json_to_sheet(this.items || []);
      const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
      const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, 'items');
    });
  }

  saveAsExcelFile(buffer: any, fileName: string): void {
    let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    let EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });
    FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  }
}
