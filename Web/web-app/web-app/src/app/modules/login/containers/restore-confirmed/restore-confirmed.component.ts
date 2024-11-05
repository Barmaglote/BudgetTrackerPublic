import { Component } from '@angular/core';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from '../../login-routing.module';

@Component({
  selector: 'app-restore-confirmed',
  templateUrl: './restore-confirmed.component.html',
  styleUrls: ['./restore-confirmed.component.css']
})
export class RestoreConfirmedComponent {
  public loginLink: string = `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.root}`;
  constructor() { }
}
