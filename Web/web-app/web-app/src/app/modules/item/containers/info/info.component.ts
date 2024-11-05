import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { Item } from '../../models';
import { BehaviorSubject, Observable, combineLatest, map } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { GarbageCollector } from 'src/app/models';
import { UserSettings } from 'src/app/models/user-settings';
import * as settingsFeatureStore from '../../../settings/store';
import { AccountItem } from 'src/app/shared/models/account-item';

@Component({
  selector: 'app-info',
  templateUrl: './info.component.html',
  styleUrls: ['./info.component.css']
})
export class InfoComponent implements OnInit, OnDestroy {
  public item$: Observable<Item | undefined> = this.store.select(featureStore.ItemSelectors.item);
  public area$: BehaviorSubject<string | undefined> = new BehaviorSubject<string | undefined>(undefined);
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(settingsFeatureStore.SettingsSelectors.userSettings);

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => {
      return (userSettings?.accounts ? JSON.parse(JSON.stringify(userSettings.accounts || [])) : []) as AccountItem[];
    })
  )

  public account$ = combineLatest([this.accounts$, this.item$]).pipe(
    map(([accounts, item]) => (!accounts || !item) ? null : accounts.find(x => x.id === item.accountId))
  )

  constructor(private store: Store, private activatedRoute: ActivatedRoute, private router: Router) { }

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.item$, this.area$, this.userSettings$
    ]);
    garbageCollector.collect();
  }

  ngOnInit(): void {
    const match = this.router.url.match(/\/user\/([^\/?]+)/);
    this.area$.next(match ? match[1] : undefined);
    const id = this.activatedRoute.snapshot.params['id'];
    this.store.dispatch(featureStore.ItemActions.getItem(id, this.area$.getValue() || ''));
  }

  navigateBack() {
    this.router.navigate(['../'], { relativeTo: this.activatedRoute });
  }
}
