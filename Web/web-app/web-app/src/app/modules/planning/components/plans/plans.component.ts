import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { PlannedItem } from 'src/app/shared/models/planned-item';

@Component({
  selector: 'app-plans',
  templateUrl: './plans.component.html',
  styleUrls: ['./plans.component.css']
})
export class PlansComponent {
  @Input() plans: PlannedItem[] | undefined = [];
  private selectedPlannedItem : PlannedItem | undefined;

  @Output() delete = new EventEmitter<PlannedItem>();
  @Output() addTransfer = new EventEmitter<PlannedItem>();
  @Output() addItem = new EventEmitter<PlannedItem>();
  @Output() edit = new EventEmitter<PlannedItem>();

  selectItem(plannedItem: PlannedItem) {
    this.selectedPlannedItem = plannedItem;
  }

  public menuItems: MenuItem[] = [
    {
      label: 'Delete item',
      icon: 'pi pi-fw pi-trash',
      command: () => { this.delete.emit(this.selectedPlannedItem) }
    },
    {
      label: 'Edit item',
      icon: 'pi pi-fw pi-pencil',
      command: () => { this.edit.emit(this.selectedPlannedItem) }
    },
    {
      label: 'Activate',
      icon: 'pi pi-fw pi-file',
      command: () => { this.addItem.emit(this.selectedPlannedItem) }
    }
  ];
}
