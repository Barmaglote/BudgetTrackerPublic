import { NgModule } from '@angular/core';
import { ItemRoutingModule } from './item-routing.module';
import { COMPONENTS } from './components';
import { SharedModule } from 'src/app/shared/shared.module';
import { CONTAINERS } from './containers';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { SpeedDialModule } from 'primeng/speeddial';
import { MessagesModule } from 'primeng/messages';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { CarouselModule } from 'primeng/carousel';
import { PanelModule } from 'primeng/panel';
import { ChipModule } from 'primeng/chip';
import { MultiSelectModule } from 'primeng/multiselect';
import { TabViewModule } from 'primeng/tabview';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { CheckboxModule } from 'primeng/checkbox';
import { DividerModule } from 'primeng/divider';
import { MessageModule } from 'primeng/message';
import { ProgressBarModule } from 'primeng/progressbar';

@NgModule({
  imports: [
    ProgressBarModule,
    MessageModule,
    DividerModule,
    CheckboxModule,
    MenuModule,
    ButtonModule,
    TieredMenuModule,
    CalendarModule,
    SpeedDialModule,
    ItemRoutingModule,
    SharedModule,
    ReactiveFormsModule,
    TableModule,
    FormsModule,
    StoreDevtoolsModule,
    MessagesModule,
    InputTextareaModule,
    InputNumberModule,
    DropdownModule,
    CardModule,
    TagModule,
    DialogModule,
    CarouselModule,
    ConfirmPopupModule,
    ConfirmDialogModule,
    InputTextModule,
    PanelModule,
    ChipModule,
    MultiSelectModule,
    TabViewModule,
    ToggleButtonModule
  ],
  exports: [
    SharedModule,
    ReactiveFormsModule,
    FormsModule,
    StoreDevtoolsModule,
    SpeedDialModule,
    InputNumberModule,
    DropdownModule,
    CarouselModule,
    ...COMPONENTS, ...CONTAINERS
  ],
  declarations: [...COMPONENTS, ...CONTAINERS],
})
export class ItemModule { }




