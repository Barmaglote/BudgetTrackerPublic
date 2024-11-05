import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreditsRoutingModule } from './credits-routing.module';
import { COMPONENTS } from './components';
import { CONTAINERS } from './containers';
import { DialogModule } from 'primeng/dialog';
import { TranslocoModule } from '@ngneat/transloco';
import { SharedModule } from 'src/app/shared/shared.module';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { TableModule } from 'primeng/table';
import { MenuModule } from 'primeng/menu';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { MessagesModule } from 'primeng/messages';
import { TagModule } from 'primeng/tag';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { DIALOGS } from './dialogs';
import { DividerModule } from 'primeng/divider';
import { AccordionModule } from 'primeng/accordion';
import { PanelModule } from 'primeng/panel';
import { MessageService } from 'primeng/api';
import { MessageModule } from 'primeng/message';
import { CREDITS_PROVIDERS } from './store';

@NgModule({
  imports: [
    MessageModule,
    AccordionModule,
    PanelModule,
    DividerModule,
    DynamicDialogModule,
    TagModule,
    MessagesModule,
    ToastModule,
    ConfirmDialogModule,
    MenuModule,
    TableModule,
    ButtonModule,
    CalendarModule,
    ReactiveFormsModule,
    FormsModule,
    InputNumberModule,
    SharedModule,
    CheckboxModule,
    InputTextareaModule,
    CommonModule,
    DialogModule,
    TranslocoModule,
    CreditsRoutingModule
  ],
  declarations: [...COMPONENTS, ...CONTAINERS, ...DIALOGS],
  providers: [MessageService, ...CREDITS_PROVIDERS]
})
export class CreditsModule { }
