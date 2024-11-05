import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { BehaviorSubject, Observable, Subscription, combineLatest, firstValueFrom, map, tap } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import * as featureStore from '../../store';
import { GarbageCollector } from 'src/app/models';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TemplateItem } from 'src/app/shared/models/template-item';
import { ActivatedRoute, NavigationEnd, Route, Router } from '@angular/router';
import { AppError } from 'src/app/core/models';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-templates',
  templateUrl: './templates.component.html',
  styleUrls: ['./templates.component.css']
})
export class TemplatesComponent implements OnDestroy, OnInit {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureStore.SettingsSelectors.userSettings);
  public selectedTemplate: TemplateItem | undefined;
  private area$: BehaviorSubject<string | undefined> = new BehaviorSubject<string | undefined>(undefined);
  public currentItemInForm: TemplateItem | undefined;

  private _templatesFormSubscription: Subscription | undefined;
  private _routerSubscription: Subscription | undefined;

  public categories$ = combineLatest([this.userSettings$, this.area$]).pipe(
    map(([userSettings, area]) => area ? (userSettings?.categories ? JSON.parse(JSON.stringify(userSettings.categories[area] || [])) : []) : [])
  )

  public templates$ = combineLatest([this.userSettings$, this.area$]).pipe(
    map(([userSettings, area]) => area ? (userSettings?.templates ? JSON.parse(JSON.stringify(userSettings.templates[area] || [])) : []) as TemplateItem[] : [])
  )

  public accounts$ = this.userSettings$.pipe(
    map((userSettings) => userSettings?.accounts || [])
  )

  public templatesForm: FormGroup = this.formBuilder.group({
    item: {},
  });

  constructor(
    private store: Store,
    private formBuilder: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private confirmationService: ConfirmationService,
    private router: Router
    ) { }

  ngOnInit(): void {
    if (this.activatedRoute.snapshot.routeConfig?.data != null) {
      const area = this.activatedRoute.snapshot.routeConfig.data["area"];
      this.area$.next(area);
    }

    this._templatesFormSubscription = this.templatesForm.valueChanges.subscribe((value) => {
      this.currentItemInForm = value.item;
    });

    this._routerSubscription = this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (this.activatedRoute.snapshot.routeConfig?.data != null) {
          if (event.url) {
            const parts = event.url.split('/');
            const area = parts[parts.length - 1];
            this.area$.next(area);
          }
        }
      }
    });
  }

  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.userSettings$, this.accounts$, this.area$, this.userSettings$, this.categories$, this.templates$
    ]);
    garbageCollector.collect();

    this._routerSubscription?.unsubscribe();
    this._templatesFormSubscription?.unsubscribe();
  }

  async onSave(){
    if (this.templatesForm.invalid) {
      this.templatesForm.markAllAsTouched();
      return;
    }

    const area = this.area$.getValue();
    if (!area) {
      throw new AppError("Area is not set");
    }

    let templates = await firstValueFrom(this.templates$);
    let userSettings = await firstValueFrom(this.userSettings$);

    let {item} = this.templatesForm.value;
    templates = templates.filter(x => x.title !== item.title);

    if (!item?.title) {
      return;
    }

    let updated = {...userSettings?.templates};
    updated[area] = [...templates, item];

    this.store.dispatch(featureStore.SettingsActions.saveTemplates(updated));
  }

  onNew(){
    this.templatesForm.setValue({ item: {} });
  }

  async onDelete(){
    let { item } = this.templatesForm.value;
    if (!item?.title) {
      return;
    }

    let templates = await firstValueFrom(this.templates$);
    let userSettings = await firstValueFrom(this.userSettings$);

    const area = this.area$.getValue();

    if (!area) {
      throw new AppError("Area is not set");
    }

    templates = templates.filter(x => x.title !== item.title);

    let updated = {...userSettings?.templates};

    updated[area] = [...templates];

    this.confirmationService.confirm({
      message: 'Are you sure you want to delete Template ' + item.title + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.store.dispatch(featureStore.SettingsActions.saveTemplates(updated));
        this.onNew();
      }
    });
  }

  onSelectTemplate() {
    this.templatesForm.setValue({ item: JSON.parse(JSON.stringify(this.selectedTemplate)) });
  }
}
