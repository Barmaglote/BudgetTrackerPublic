import { Component, OnInit } from '@angular/core';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from '../../login-routing.module';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-restore',
  templateUrl: './restore.component.html',
  styleUrls: ['./restore.component.css']
})
export class RestoreComponent implements OnInit {
  public loginLink: string = `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.root}`;
  private token: string | undefined | null = undefined;
  private login: string | undefined | null = undefined;

  constructor(private store: Store, private activatedRoute: ActivatedRoute) { }
  ngOnInit(): void {
    this.token = this.activatedRoute.snapshot.queryParams['token'];
    this.login = this.activatedRoute.snapshot.queryParams['login'];
  }

  onSubmit(event: any) {

    let { password } = event;
    if (!password || !this.token || !this.login) { return; }

    this.store.dispatch(featureStore.LoginActions.restore(password, this.token, this.login));
  }
}
