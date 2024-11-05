import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlanningRoutingModule } from './planning-routing.module';
import { CardModule } from 'primeng/card';
import { SharedModule } from 'src/app/shared/shared.module';
import { PanelModule } from 'primeng/panel';
import { MessagesModule } from 'primeng/messages';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MenuModule } from 'primeng/menu';
import { PanelMenuModule } from 'primeng/panelmenu';
import { DropdownModule } from 'primeng/dropdown';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { PLANNING_COMPONENTS } from './components';
import { PLANNING_CONTAINERS } from './containers';
import { CalendarModule } from 'primeng/calendar';
import { TabViewModule } from 'primeng/tabview';
import { InputNumberModule } from 'primeng/inputnumber';
import { PLANNING_PROVIDERS } from './store';
import { DIALOGS } from './dialogs';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ToastModule } from 'primeng/toast';
import { TagModule } from 'primeng/tag';

@NgModule({
  imports: [
    TagModule,
    InputNumberModule,
    CalendarModule,
    TabViewModule,
    ConfirmPopupModule,
    ConfirmDialogModule,
    DropdownModule,
    PanelMenuModule,
    MenuModule,
    CommonModule,
    PlanningRoutingModule,
    CardModule,
    SharedModule,
    PanelModule,
    MessagesModule,
    FormsModule,
    ReactiveFormsModule,
    InputTextareaModule,
    ToastModule
  ],
  declarations: [...PLANNING_COMPONENTS, ...PLANNING_CONTAINERS, ...DIALOGS],
  providers: [PLANNING_PROVIDERS, ConfirmationService],
})
export class PlanningModule { }
