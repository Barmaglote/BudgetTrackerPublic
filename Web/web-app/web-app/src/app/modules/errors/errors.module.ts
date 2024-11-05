import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorsRoutingModule } from './errors-routing.module';
import { ButtonModule } from 'primeng/button';
import { SharedModule } from 'src/app/shared/shared.module';
import { CONTAINERS } from './containers';

@NgModule({
  imports: [
    CommonModule,
    ErrorsRoutingModule,
    ButtonModule,
    SharedModule
  ],
  declarations: [...CONTAINERS],
})
export class ErrorsModule { }
