import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UPGRADE_CONTAINERS } from './containers';
import { UPGRADE_COMPONENTS } from './components';
import { CardModule } from 'primeng/card';
import { SharedModule } from 'src/app/shared/shared.module';
import { PanelModule } from 'primeng/panel';
import { MessagesModule } from 'primeng/messages';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { UpgradeRoutingModule } from './upgrade-routing.module';
import { TagModule } from 'primeng/tag';

@NgModule({
  imports: [
    CommonModule,
    UpgradeRoutingModule,
    CardModule,
    SharedModule,
    PanelModule,
    MessagesModule,
    FormsModule,
    ReactiveFormsModule,
    TagModule
  ],
  declarations: [...UPGRADE_CONTAINERS, ...UPGRADE_COMPONENTS],
  providers: [ConfirmationService],
})
export class UpgradeModule { }
