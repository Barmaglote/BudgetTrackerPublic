import { Component, Input, OnInit, signal } from '@angular/core';
import { AccountItem } from 'src/app/shared/models/account-item';

interface CurrencyValue {
  currency: string,
  quantitySum: number
}

@Component({
  selector: 'app-account-structure',
  templateUrl: './account-structure.component.html',
  styleUrls: ['./account-structure.component.css']
})
export class AccountStructureComponent {
  private _items: AccountItem[] = [];

  @Input() get items(): AccountItem[]{
    return this._items;
  }

  set items(value) {
    this._items = value;

    if (!value) {
      this.aggregatedData.set([]);
      return;
    }

    let aggregatedData: { [key: string]: number } = {};
    value.forEach((item: AccountItem) => {
      if (aggregatedData[item.currency]) {
        aggregatedData[item.currency] += item.quantity;
      } else {
        aggregatedData[item.currency] = item.quantity;
      }
    });

    this.aggregatedData.set(Object.keys(aggregatedData).map(x => ({currency: x, quantitySum: aggregatedData[x]})));
  }

  public aggregatedData = signal<CurrencyValue[]>([]);
}
