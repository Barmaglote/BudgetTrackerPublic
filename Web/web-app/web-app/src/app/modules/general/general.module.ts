import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GeneralRoutingModule } from './general-routing.module';
import { ButtonModule } from 'primeng/button';
import { SharedModule } from 'src/app/shared/shared.module';
import { GoogleSigninButtonModule, SocialLoginModule } from '@abacritt/angularx-social-login';
import { CONTAINERS } from './containers';

@NgModule({
  imports: [
    CommonModule,
    GeneralRoutingModule,
    ButtonModule,
    SharedModule,
    SocialLoginModule,
    GoogleSigninButtonModule
  ],
  declarations: [...CONTAINERS],
})
export class GeneralModule { }
