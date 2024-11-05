import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { TranslocoModule } from '@ngneat/transloco';
import { COMPONENTS } from './components';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { SkeletonModule } from 'primeng/skeleton';
import { ChartModule } from 'primeng/chart';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { PanelMenuModule } from 'primeng/panelmenu';
import { CarouselModule } from 'primeng/carousel';
import { DIALOGS } from './dialogs';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DividerModule } from 'primeng/divider';
import { CalendarModule } from 'primeng/calendar';
import { ClipboardModule } from '@angular/cdk/clipboard';
import { DIRECTIVES } from './directives';

@NgModule({
  imports: [
    CarouselModule,
    PanelMenuModule,
    DropdownModule,
    SkeletonModule,
    BreadcrumbModule,
    ButtonModule,
    CommonModule,
    FormsModule,
    InputTextModule,
    StoreDevtoolsModule,
    TranslocoModule,
    ChartModule,
    DialogModule,
    InputNumberModule,
    ReactiveFormsModule,
    CheckboxModule,
    InputTextareaModule,
    DividerModule,
    CalendarModule,
    ClipboardModule
  ],
  exports: [
    DropdownModule,
    SkeletonModule,
    ButtonModule,
    CommonModule,
    FormsModule,
    InputTextModule,
    StoreDevtoolsModule,
    TranslocoModule,
    DialogModule,
    ClipboardModule,
    ...COMPONENTS
  ],
  declarations: [...COMPONENTS, ...DIALOGS, ...DIRECTIVES],
})
export class SharedModule { }
