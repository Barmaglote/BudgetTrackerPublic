import { Component } from '@angular/core';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from '../../login-routing.module';

@Component({
  selector: 'app-restore-sent',
  templateUrl: './restore-sent.component.html',
  styleUrls: ['./restore-sent.component.css']
})
export class RestoreSentComponent {
  public loginLink: string = `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.root}`;
  constructor() { }
}
