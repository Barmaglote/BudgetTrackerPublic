import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FilterMetadata } from 'primeng/api';
import { Category } from 'src/app/models/category';
import { Filter } from 'src/app/models/filter';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.css']
})
export class FilterComponent {
  @Output() search = new EventEmitter<Filter>();
  @Input({required: true}) categories: Category[] = [];
  @Input({required: true}) accounts: AccountItem[] = [];

  public selectedCategories!: Category[];
  public selectedAccounts!: AccountItem[];
  public comment: string | undefined = undefined;
  public rangeDates: Date[] | undefined;
  public minDate: Date = new Date(new Date().setFullYear(new Date().getFullYear() - 1));

  constructor() { }

  onSearch(){
    this.search.emit({
      ...this.getCommentsFilter(),
      ...this.getDatesFilter(),
      ...this.getCategoriesFilter(),
      ...this.getAccountsFilter()
    });
  }

  onReset(){
    this.rangeDates = undefined;
    this.comment = undefined;
    this.selectedCategories = [];
    this.onSearch();
  }

  getCategoriesFilter() {
    let filterMetadata: FilterMetadata[] = [];
    this.selectedCategories?.forEach(category => {
      filterMetadata.push({
        value: category.category,
        matchMode: 'eq',
        operator: 'or'
      })
    });

    const customFilters = {
      'category': this.selectedCategories ? filterMetadata : undefined,
    };

    return customFilters;
  }

  getAccountsFilter() {
    let filterMetadata: FilterMetadata[] = [];
    this.selectedAccounts?.forEach(account => {
      filterMetadata.push({
        value: account.id,
        matchMode: 'eq',
        operator: 'or'
      })
    });

    const customFilters = {
      'accountId': this.selectedAccounts ? filterMetadata : undefined,
    };

    return customFilters;
  }

  getCommentsFilter() {
    const filterMetadata = { value: this.comment, matchMode: 'contains', operator: 'and' } as FilterMetadata;

    const customFilters = {
      'comment': this.comment ? [filterMetadata] : undefined,
    };

    return customFilters;
  }

  getDatesFilter() {
    let filterMetadata;
    if (this.rangeDates) {
      filterMetadata = [
        {
          value: this.rangeDates[0],
          matchMode: 'gte',
          operator: 'and'
        },
        {
          value: this.rangeDates[1],
          matchMode: 'lte',
          operator: 'and'
        }
      ] as FilterMetadata[];
    }

    const customFilters = {
      'date': this.rangeDates ? filterMetadata : undefined,
    };

    return customFilters;
  }
}
