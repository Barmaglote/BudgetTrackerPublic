import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Table, TableLazyLoadEvent } from 'primeng/table';
import { FilterMetadata, MenuItem } from 'primeng/api';
import { Category } from 'src/app/models/category';
import { TransferItem } from 'src/app/shared/models/transfer-item';

@Component({
  selector: 'app-transactions-table',
  templateUrl: './transactions-table.component.html',
  styleUrls: ['./transactions-table.component.css'],
})
export class TransactionsTableComponent {
  @ViewChild('table') table!: Table;
  @Input({required: true}) items: TransferItem[] | null | undefined = [];
  @Input({required: true}) totalRecords: number = 0;
  @Input({required: true}) title!: string;
  @Output() loadData = new EventEmitter<TableLazyLoadEvent>();
  @Output() rockback = new EventEmitter<TransferItem>();
  @Output() info = new EventEmitter<TransferItem>();
  public Math = Math;

  public itemDialog: boolean = false;
  public item!: TransferItem;
  public category: Category | undefined;
  public submitted: boolean = false;
  public filterMetadata = { value: '', matchMode: 'contains', operator: 'comment' } as FilterMetadata;
  public comment: string | undefined = undefined;
  public rangeDates: Date[] | undefined;
  public isGrouped: boolean = false;
  public onIconClass: string ="pi pi-minus-circle";
  public offIconClass: string ="pi pi-plus-circle";

  private currentItem!: TransferItem;

  selectItem(item: TransferItem) {
    this.currentItem = item;
  }

  public menuItems: MenuItem[] = [
    {
      label: 'Info',
      icon: 'pi pi-info',
      command: () => { this.infoItem(this.currentItem) }
    },
    {
      label: 'Rollback',
      icon: 'pi pi-trash',
      command: () => { this.rollbackItem(this.currentItem) }
    },
  ]

  constructor() { }

  onLoadData(event: TableLazyLoadEvent) {
    this.loadData.emit(event);
  }

  infoItem(item: TransferItem) {
    this.info.emit(item);
  }

  rollbackItem(item: TransferItem) {
    this.rockback.emit(item);
  }

  hideDialog() {
    this.itemDialog = false;
    this.submitted = false;
  }
}
