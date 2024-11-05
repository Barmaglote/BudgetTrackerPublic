import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { LoaderService } from 'src/app/core/services';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss'],
})
export class LoaderComponent implements OnDestroy {
  public loading: boolean = false;
  private _loaderServiceSubscription: Subscription | undefined;

  constructor(private loaderService: LoaderService) {
    this._loaderServiceSubscription = this.loaderService.isLoading.subscribe((v) => {
      this.loading = v;
    });
  }

  ngOnDestroy() {
    if (this._loaderServiceSubscription) {
      this._loaderServiceSubscription.unsubscribe();
    }
  }
}
