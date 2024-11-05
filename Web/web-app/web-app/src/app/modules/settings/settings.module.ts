import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SETTINGS_CONTAINERS } from './containers';
import { SETTINGS_COMPONENTS } from './components';
import { SettingRoutingModule } from './settings-routing.module';
import { AccordionModule } from 'primeng/accordion';
import { ChipModule } from 'primeng/chip';
import { CardModule } from 'primeng/card';
import { SharedModule } from 'src/app/shared/shared.module';
import { ColorPickerModule } from 'primeng/colorpicker';
import { PanelModule } from 'primeng/panel';
import { MessagesModule } from 'primeng/messages';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PickListModule } from 'primeng/picklist';
import { DragDropModule } from 'primeng/dragdrop';
import { MenuModule } from 'primeng/menu';
import { PanelMenuModule } from 'primeng/panelmenu';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { ListboxModule } from 'primeng/listbox';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { CheckboxModule } from 'primeng/checkbox';
import { SliderModule } from 'primeng/slider';

@NgModule({
  imports: [
    SliderModule,
    CheckboxModule,
    ConfirmPopupModule,
    ConfirmDialogModule,
    ListboxModule,
    InputTextareaModule,
    DropdownModule,
    PanelMenuModule,
    MenuModule,
    DragDropModule,
    PickListModule,
    CommonModule,
    SettingRoutingModule,
    AccordionModule,
    ChipModule,
    CardModule,
    SharedModule,
    ColorPickerModule,
    PanelModule,
    MessagesModule,
    FormsModule,
    ReactiveFormsModule,
    InputNumberModule
  ],
  declarations: [...SETTINGS_COMPONENTS, ...SETTINGS_CONTAINERS],
  providers: [ConfirmationService],
})
export class SettingsModule { }
