import { ChangePasswordRequestFormComponent } from "./change-password-request-form/change-password-request-form.component";
import { ConfirmationFormComponent } from "./confirmation-form/confirmation-form.component";
import { LoginFormComponent } from "./login-form/login-form.component";
import { RestoreFormComponent } from "./restore-form/restore-form.component";
import { RestoreRequestFormComponent } from "./restore-request-form/restore-request-form.component";
import { SignupFormComponent } from "./signup-form/signup-form.component";

export const COMPONENTS = [
  ConfirmationFormComponent,
  SignupFormComponent,
  LoginFormComponent,
  RestoreRequestFormComponent,
  RestoreFormComponent,
  ChangePasswordRequestFormComponent
];

export * from "./confirmation-form/confirmation-form.component";
export * from "./signup-form/signup-form.component";
export * from "./login-form/login-form.component";
export * from "./restore-request-form/restore-request-form.component";
export * from "./restore-form/restore-form.component";
export * from "./change-password-request-form/change-password-request-form.component";
