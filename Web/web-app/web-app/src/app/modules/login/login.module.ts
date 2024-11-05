import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginRoutingModule } from './login-routing.module';
import { DividerModule } from 'primeng/divider';
import { CheckboxModule } from 'primeng/checkbox';
import { CarouselModule } from 'primeng/carousel';
import { SharedModule } from 'src/app/shared/shared.module';
import { TagModule } from 'primeng/tag';
import { SERVICES } from './services';
import { CONTAINERS } from './containers';
import { COMPONENTS } from './components';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GoogleSigninButtonModule, SocialLoginModule } from '@abacritt/angularx-social-login';
import { ReCaptchaV3Service, RecaptchaV3Module } from 'ng-recaptcha';
import { RecaptchaModule, RecaptchaFormsModule } from "ng-recaptcha";

@NgModule({
  imports: [
    CommonModule,
    LoginRoutingModule,
    DividerModule,
    CheckboxModule,
    CarouselModule,
    SharedModule,
    TagModule,
    SocialLoginModule,
    GoogleSigninButtonModule,
    CheckboxModule,
    SharedModule,
    ReactiveFormsModule,
    FormsModule,
    RecaptchaV3Module,
    RecaptchaModule,
    RecaptchaFormsModule
  ],
  declarations: [...CONTAINERS, ...COMPONENTS],
  providers: [...SERVICES, ReCaptchaV3Service],
})
export class LoginModule { }
