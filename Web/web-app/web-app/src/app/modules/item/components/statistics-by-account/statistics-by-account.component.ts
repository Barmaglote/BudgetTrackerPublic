import { Component, Input } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { Category } from 'src/app/models/category';
import { StatisticsByCategory } from 'src/app/models/statistics-by-category';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  selector: 'app-statistics-by-account',
  templateUrl: './statistics-by-account.component.html',
  styleUrls: ['./statistics-by-account.component.css']
})
export class StatisticsByAccountComponent {
  @Input({required: true}) accounts: AccountItem[] | [] = [];

  @Input() statistics$: Observable<StatisticsByCategory[] | undefined> = new BehaviorSubject<StatisticsByCategory[] | undefined>([]);
  @Input({required: true}) categories$: Observable<Category[] | undefined> = new BehaviorSubject<Category[] | undefined>([]);

  public responsiveOptions = [
    {
        breakpoint: '1199px',
        numVisible: 1,
        numScroll: 1
    },
    {
        breakpoint: '991px',
        numVisible: 2,
        numScroll: 1
    },
    {
        breakpoint: '767px',
        numVisible: 1,
        numScroll: 1
    }
  ];

  filterByAccountId(data: StatisticsByCategory[] | undefined, accountId: string) {
    if (!data || accountId) { return of([]); }

    return of(data.filter(x => x.accountId === accountId));
  }
}
