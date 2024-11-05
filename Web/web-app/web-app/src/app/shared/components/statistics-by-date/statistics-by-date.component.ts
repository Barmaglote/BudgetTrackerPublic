import { Component, Input, OnDestroy, OnInit, signal } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, combineLatest, map } from 'rxjs';
import { Category } from 'src/app/models/category';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';

interface CategoryDictionary {
  [date: string]: any;
}

@Component({
  selector: 'app-statistics-by-date',
  templateUrl: './statistics-by-date.component.html',
  styleUrls: ['./statistics-by-date.component.css']
})
export class StatisticsByDateComponent implements OnInit, OnDestroy {
  ngOnDestroy(): void {
    if (this._statisticsCategoriesSubscription) {
      this._statisticsCategoriesSubscription.unsubscribe();
    }
  }
  @Input() Statistics$: Observable<StatisticsByDate[] | undefined> = new BehaviorSubject<StatisticsByDate[] | undefined>([]);
  @Input() Categories$: Observable<Category[] | undefined> = new BehaviorSubject<Category[] | undefined>([]);

  public isVisible = signal<boolean>(false);
  public isFullViewVisible: boolean = false;
  private _statisticsCategoriesSubscription: Subscription | undefined;

  data: any;
  options: any;

  ngOnInit() {
    this._statisticsCategoriesSubscription = combineLatest([this.Statistics$, this.Categories$]).pipe(
      map(([Statistics, Categories]) => {
        this.isVisible.set(false);

        const documentStyle = getComputedStyle(document.documentElement);
        const textColor = documentStyle.getPropertyValue('--text-color'); // ?
        const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
        const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

        this.data = this.transformData(Statistics, Categories);

        this.options = {
          maintainAspectRatio: false,
          responsive: true,
          aspectRatio: 1,
          plugins: {
              tooltips: {
                  mode: 'index',
                  intersect: false
              },
              legend: {
                  labels: {
                      color: textColor
                  }
              }
          },
          scales: {
              x: {
                  stacked: true,
                  ticks: {
                      color: textColorSecondary
                  },
                  grid: {
                      color: surfaceBorder,
                      drawBorder: false
                  }
              },
              y: {
                  stacked: true,
                  ticks: {
                      color: textColorSecondary
                  },
                  grid: {
                      color: surfaceBorder,
                      drawBorder: false
                  }
              }
          }
        };

        this.isVisible.set(true);
      })
    ).subscribe();
  }

  private transformData(items: StatisticsByDate[] | undefined | null, categories: Category[] | undefined): any {
    if (!items || !categories) return null;
    let labels: string[] = [];
    let dataset: CategoryDictionary = {};

    let dates = items.map((stat : StatisticsByDate) => new Date(stat.date).getTime());
    let minDate = new Date(Math.min(...dates));
    let maxDate = new Date(Math.max(...dates));

    const dateArray: string[] = [];
    let currentDate = new Date(minDate);

    while (currentDate <= maxDate) {
      const formattedDate = currentDate.toISOString().slice(0, 7); // "YYYY-MM"
      dateArray.push(formattedDate);

      currentDate.setMonth(currentDate.getMonth() + 1);
    }

    dateArray.forEach(item => {
      labels.push(item);
      categories.forEach(cat => {
        if (!cat.category) return;

        if (!dataset[cat.category]) {
          dataset[cat?.category] = {
            type: 'bar',
            label: cat?.category,
            backgroundColor: cat?.color,
            data: [items.filter(x => x.category === cat?.category && new Date(x.date).toISOString().slice(0, 7) === item).reduce((accumulator, currentValue) => accumulator + currentValue.quantity, 0)]
          }
        } else {
          let category = dataset[cat?.category];
          category['data'].push(items.filter(x => x.category === cat?.category && new Date(x.date).toISOString().slice(0, 7) === item).reduce((accumulator, currentValue) => accumulator + currentValue.quantity, 0));
          dataset[cat?.category] = category;
        }
      });
    });

    return {
      labels: labels,
      datasets: Object.values(dataset)
    };
  }
}
