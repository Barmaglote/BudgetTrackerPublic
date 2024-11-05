import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainRoutingModule } from './main-routing.module';
import { SharedModule as CustomSharedModule } from './../../shared/shared.module';
import { MenubarModule } from 'primeng/menubar';
import { MenuModule } from 'primeng/menu';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { PanelMenuModule } from 'primeng/panelmenu';
import { CONTAINERS } from './containers';
import { COMPONENTS } from './components';
import { SplitButtonModule } from 'primeng/splitbutton';
import { SidebarModule } from 'primeng/sidebar';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { ProgressBarModule } from 'primeng/progressbar';

@NgModule({
  imports: [
    AvatarModule,
    AvatarGroupModule,
    CommonModule,
    SplitButtonModule,
    PanelMenuModule,
    MainRoutingModule,
    CustomSharedModule,
    MenubarModule,
    FormsModule,
    InputTextModule,
    MenuModule,
    ReactiveFormsModule,
    SidebarModule,
    ProgressBarModule
  ],
  exports: [...CONTAINERS,  ...COMPONENTS],
  declarations: [...CONTAINERS, ...COMPONENTS]
})
export class MainModule { }

