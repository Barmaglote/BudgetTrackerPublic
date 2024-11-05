import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css'],
})
export class SignupComponent {

  constructor(private store: Store) { }

  onSubmit(event: any) {
    let { title, email, password, recaptcha } = event;
    if (!email) { return; }

    this.store.dispatch(featureStore.LoginActions.signUp(title, email, password, recaptcha));
  }
}
