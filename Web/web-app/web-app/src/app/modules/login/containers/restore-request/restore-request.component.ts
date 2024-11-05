import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from '../../login-routing.module';

@Component({
  selector: 'app-restore-request',
  templateUrl: './restore-request.component.html',
  styleUrls: ['./restore-request.component.css'],
})
export class RestoreRequestComponent {
  public signUpLink: string = `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.signup}`;
  public loginLink: string = `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.root}`;

  constructor(private store: Store) { }

  onSubmit(event: any) {
    let { email, recaptcha } = event;
    if (!email || !recaptcha) { return; }

    this.store.dispatch(featureStore.LoginActions.restoreRequest(email, recaptcha));
  }
}
