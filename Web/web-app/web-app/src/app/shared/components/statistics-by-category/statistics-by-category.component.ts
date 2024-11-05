import { Component, Input, OnDestroy, OnInit, signal } from '@angular/core';
import { BehaviorSubject, combineLatest, map, Observable, Subscription } from 'rxjs';
import { Category } from 'src/app/models/category';
import { StatisticsByCategory } from 'src/app/models/statistics-by-category';

interface CategoryDictionary {
  [category: string]: Category;
}

@Component({
  selector: 'app-statistics-by-category',
  templateUrl: './statistics-by-category.component.html',
  styleUrls: ['./statistics-by-category.component.css']
})
export class StatisticsByCategoryComponent implements OnInit, OnDestroy {

  ngOnDestroy(): void {
    if (this._statisticsCategoriesSubscription) {
      this._statisticsCategoriesSubscription.unsubscribe();
    }
  }

  @Input() Statistics$: Observable<StatisticsByCategory[] | undefined> = new BehaviorSubject<StatisticsByCategory[] | undefined>([]);
  @Input({required: true}) Categories$: Observable<Category[] | undefined> = new BehaviorSubject<Category[] | undefined>([]);

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

        this.data = this.transformData(Statistics, Categories);

        this.options = {
          maintainAspectRatio: false,
          responsive: true,
          aspectRatio: 1,
          plugins: {
            legend: {
              labels: {
                color: textColor
              }
            }
          }
        }

        this.isVisible.set(true);
      })
    ).subscribe();
  }

  private transformData(data: StatisticsByCategory[] | undefined | null, categories: Category[] | undefined): any {

    if (!data || !categories) return null;
    const labels: string[] = [];
    const dataset = {
      data: [] as number[],
      backgroundColor: [] as string[],
      hoverBackgroundColor: [] as string[]
    };

    const categoryDictionary: CategoryDictionary = {};

    categories?.forEach(item => {
      categoryDictionary[item.category] = item;
    });

    data.forEach(item => {
      labels.push(item.category || 'undefined');
      dataset.data.push(item.quantity);
      dataset.backgroundColor = data.map(x => categoryDictionary[x.category]?.color);
      dataset.hoverBackgroundColor = data.map(x => categoryDictionary[x.category]?.hoverColor);
    });

    return {
      labels: labels,
      datasets: [dataset]
    };
  }
}
