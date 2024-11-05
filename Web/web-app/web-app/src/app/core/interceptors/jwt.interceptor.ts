import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthenticationService } from '../services';
import { SocialUser } from '@abacritt/angularx-social-login';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const currentUser: SocialUser | undefined = this.authenticationService.getUser();

    console.log("intercept JWT 01")

    const isLoggedIn = currentUser && currentUser.idToken;
    console.log("intercept JWT 02", {isLoggedIn});
    const isApiUrl = request.url.startsWith(environment.webapi) || request.url.startsWith(environment.paymentapi);
    console.log("intercept JWT 02", {isApiUrl});
    const isLoginApiUrl = request.url.startsWith(environment.loginapi);
    console.log("intercept JWT 03", {isLoginApiUrl});
    if (isLoggedIn && isApiUrl) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.idToken}`,
          Provider: currentUser.provider
        },
      });
    }

    if (isLoggedIn && isLoginApiUrl) {
      request = request.clone({
        setHeaders: {
          'x-access-token': currentUser?.authToken,
          Authorization: `Bearer ${currentUser.idToken}`,
          Provider: currentUser.provider
        },
      });
    }

    console.log("intercept JWT 04", {request});

    return next.handle(request);
  }
}
