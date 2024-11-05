import { Component, Input, OnDestroy, OnInit, signal } from '@angular/core';
import { BehaviorSubject, Observable, map, combineLatest, Subscription } from 'rxjs';

import { StatisticsByDate } from 'src/app/models/statistics-by-date';

@Component({
  selector: 'app-statistics-by-date-plain',
  templateUrl: './statistics-by-date-plain.component.html',
  styleUrls: ['./statistics-by-date-plain.component.css']
})
export class StatisticsByDatePlainComponent implements OnInit, OnDestroy {
  ngOnDestroy(): void {
    if (this._statisticsCategoriesSubscription) {
      this._statisticsCategoriesSubscription.unsubscribe();
    }
  }
  @Input() statisticsIncome$: Observable<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  @Input() statisticsExpenses$: Observable<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  public isVisible = signal<boolean>(false);
  private _statisticsCategoriesSubscription: Subscription | undefined;
  data: any;
  options: any;

  ngOnInit() {
    this._statisticsCategoriesSubscription = combineLatest([this.statisticsIncome$,this.statisticsExpenses$])
    .pipe(
      map(([statisticsIncome, statisticsExpenses]) => {
        this.isVisible.set(false);
        this.data = this.transformData(statisticsIncome, statisticsExpenses);

        this.options = {
          responsive: true,
          maintainAspectRatio: false,
          aspectRatio: 3,
          plugins: {
              tooltips: {
                  mode: 'index',
                  intersect: false
              },
              legend: {
                  display: false,
              }
          },
          scales: {
              x: {
                  display: false,
              },
              y: {
                  display: false,
              }
          }
        };

        this.isVisible.set(true);
      })
    ).subscribe();
  }

  private transformData(income: StatisticsByDate[] | undefined | null, expenses: StatisticsByDate[] | undefined | null): any {
    if (!income || !expenses) return null;
    let labels: string[] = [];
    const documentStyle = getComputedStyle(document.documentElement);

    let datesIncome = income.map((stat : StatisticsByDate) => new Date(stat.date).getTime());
    let datesExpenses = expenses.map((stat : StatisticsByDate) => new Date(stat.date).getTime());
    let minDate = new Date(Math.min(...datesIncome, ...datesExpenses));
    let maxDate = new Date(Math.max(...datesIncome, ...datesExpenses));

    const dateArray: string[] = [];
    let currentDate = new Date(minDate);

    while (currentDate <= maxDate) {
      const formattedDate = currentDate.toISOString().slice(0, 7); // "YYYY-MM-DD"
      dateArray.push(formattedDate);
      currentDate.setMonth(currentDate.getMonth() + 1);
    }

    var data: any[] = [];

    dateArray.forEach(item => {
      labels.push(item);
      data.push(
        income.filter(x => new Date(x.date).toISOString().slice(0, 7) === item).reduce((accumulator, currentValue) => accumulator + currentValue.quantity, 0) -
        expenses.filter(x => new Date(x.date).toISOString().slice(0, 7) === item).reduce((accumulator, currentValue) => accumulator + currentValue.quantity, 0)
      )
    });

    const datasets = [
      {
        data,
        fill: false,
        borderColor: documentStyle.getPropertyValue('--blue-300'),
        tension: 0.6,
        pointStyle: false,
      }
    ];


    return {
      labels,
      datasets
    };
  }
}
