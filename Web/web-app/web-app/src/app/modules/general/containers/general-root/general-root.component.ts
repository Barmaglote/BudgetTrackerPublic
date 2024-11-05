import { SocialUser } from '@abacritt/angularx-social-login';
import { Component } from '@angular/core';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-general-root',
  templateUrl: './general-root.component.html',
  styleUrls: ['./general-root.component.css']
})
export class GeneralRootComponent {
  public get user(): SocialUser | undefined {
    return this.authenticationService.getUser();
  }

  constructor(private authenticationService: AuthenticationService) {}
  environment = environment;

  public logout(){
    this.authenticationService.logout();
  }
}
