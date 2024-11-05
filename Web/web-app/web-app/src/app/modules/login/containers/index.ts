import { LoginComponent } from "./login/login.component";
import { SignupConfirmedComponent } from "./signup-confirmed/signup-confirmed.component";
import { SignupComponent } from "./signup/signup.component";
import { ConfirmationComponent } from "./confirmation/confirmation.component";
import { DeleteComponent } from "./delete/delete.component";
import { RestoreSentComponent } from "./restore-sent/restore-sent.component";
import { RestoreRequestComponent } from "./restore-request/restore-request.component";
import { RestoreComponent } from "./restore/restore.component";
import { RestoreConfirmedComponent } from "./restore-confirmed/restore-confirmed.component";
import { ChangePasswordRequestComponent } from "./change-password-request/change-password-request.component";
import { ChangePasswordRequestSentComponent } from "./change-password-request-sent/change-password-request-sent.component";

export const CONTAINERS = [
  LoginComponent,
  SignupComponent,
  SignupConfirmedComponent,
  ConfirmationComponent,
  DeleteComponent,
  RestoreSentComponent,
  RestoreRequestComponent,
  RestoreComponent,
  RestoreConfirmedComponent,
  ChangePasswordRequestComponent,
  ChangePasswordRequestSentComponent
];

export * from "./login/login.component";
export * from "./signup/signup.component";
export * from "./signup-confirmed/signup-confirmed.component";
export * from "./confirmation/confirmation.component";
export * from "./delete/delete.component";
export * from "./restore-sent/restore-sent.component";
export * from "./restore-request/restore-request.component";
export * from "./restore/restore.component";
export * from "./restore-confirmed/restore-confirmed.component";
export * from "./change-password-request/change-password-request.component";
export * from "./change-password-request-sent/change-password-request-sent.component";
