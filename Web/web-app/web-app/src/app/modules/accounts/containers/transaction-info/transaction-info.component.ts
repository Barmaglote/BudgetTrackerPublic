import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import * as featureStore from '../../store';
import { ActivatedRoute } from '@angular/router';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-transaction-info',
  templateUrl: './transaction-info.component.html',
  styleUrls: ['./transaction-info.component.css']
})
export class TransactionInfoComponent implements OnInit, OnDestroy {
  public transfer$: Observable<TransferItem | undefined> = this.store.select(featureStore.AccountsSelectors.transfer);

  constructor(private store: Store, private activatedRoute: ActivatedRoute) {}
  ngOnInit(): void {
    const id = this.activatedRoute.snapshot.params['id'];
    this.store.dispatch(featureStore.AccountsActions.getTransaction(id));
  }

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.transfer$
    ]);
    garbageCollector.collect();
  }
}
