import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsRoutingModule } from './reports-routing.module';
import { COMPONENTS } from './components';
import { CONTAINERS } from './containers';
import { SharedModule } from 'src/app/shared/shared.module';
import { CalendarModule } from 'primeng/calendar';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';

@NgModule({
  imports: [
    TooltipModule,
    CalendarModule,
    TableModule,
    CommonModule,
    ReportsRoutingModule,
    SharedModule
  ],
  declarations: [...COMPONENTS, ...CONTAINERS],
})
export class ReportsModule { }
