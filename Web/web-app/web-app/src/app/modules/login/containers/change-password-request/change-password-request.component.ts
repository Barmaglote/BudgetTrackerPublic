import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { AuthenticationService } from 'src/app/core/services';

@Component({
  selector: 'app-change-password-request',
  templateUrl: './change-password-request.component.html',
  styleUrls: ['./change-password-request.component.css']
})
export class ChangePasswordRequestComponent implements OnInit {
  private email: string | undefined | null = undefined;
  constructor(private store: Store, private authenticationService: AuthenticationService) {}

  ngOnInit() {
    this.email = this.authenticationService.getUser()?.email;
  }

  onSubmit(event: any) {
    let { oldpassword, newpassword } = event;
    if (!this.email || !oldpassword || !newpassword) { return; }

    this.store.dispatch(featureStore.LoginActions.changePasswordRequest(this.email, oldpassword, newpassword));
  }
}
