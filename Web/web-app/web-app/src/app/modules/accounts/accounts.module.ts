import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { SharedModule } from 'src/app/shared/shared.module';
import { PanelModule } from 'primeng/panel';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { CONTAINERS } from './containers';
import { COMPONENTS } from './components';
import { AccountsRoutingModule } from './accounts-routing.module';
import { MenuModule } from 'primeng/menu';
import { DividerModule } from 'primeng/divider';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { CarouselModule } from 'primeng/carousel';
import { TabViewModule } from 'primeng/tabview';
import { DataViewModule } from 'primeng/dataview';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { ProgressBarModule } from 'primeng/progressbar';
import { ToastModule } from 'primeng/toast';
import { TagModule } from 'primeng/tag';
import { ItemService } from './services';
import { CreditsService } from '../credits/services';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';

@NgModule({
  imports: [
    ButtonModule,
    TagModule,
    ToastModule,
    ProgressBarModule,
    DataViewModule,
    InputNumberModule,
    CommonModule,
    DividerModule,
    AccountsRoutingModule,
    CardModule,
    SharedModule,
    PanelModule,
    MenuModule,
    FormsModule,
    DropdownModule,
    ReactiveFormsModule,
    CarouselModule,
    TabViewModule,
    ConfirmDialogModule,
    TableModule,
    RippleModule
  ],
  declarations: [...COMPONENTS, ...CONTAINERS],
  providers: [ConfirmationService, ItemService, CreditsService],
})
export class AccountsModule { }
