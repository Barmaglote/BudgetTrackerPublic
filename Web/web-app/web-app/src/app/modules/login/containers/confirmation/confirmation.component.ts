import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css']
})
export class ConfirmationComponent implements OnInit {

  private token: string | undefined | null = undefined;

  constructor(private store: Store, private activatedRoute: ActivatedRoute) { }
  ngOnInit(): void {
    this.token = this.activatedRoute.snapshot.queryParams['token'];
  }

  onSubmit(event: any) {
    let { email } = event;
    if (!email) { return; }

    this.store.dispatch(featureStore.LoginActions.confirmation(email, this.token));
  }
}
