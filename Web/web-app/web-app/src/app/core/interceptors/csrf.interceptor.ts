import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpXsrfTokenExtractor } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable()
export class CSRFInterceptor implements HttpInterceptor {
  constructor(private tokenExtractor: HttpXsrfTokenExtractor) { }

  private actions: string[] = ["POST", "PUT", "DELETE"];
  private forbiddenActions: string[] = ["HEAD", "OPTIONS"];

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    console.log("intercept csrf 01");
    let token = this.tokenExtractor.getToken();
    console.log("intercept csrf 02", {token});
    let permitted = this.findByActionName(request.method, this.actions);
    let forbidden = this.findByActionName(request.method, this.forbiddenActions);;
    console.log("intercept csrf 03", {permitted, forbidden});

    if (permitted !== undefined && forbidden === undefined && token !== null) {
      request = request.clone({ setHeaders: { "X-CSRF-TOKEN": token }, withCredentials: true });
    }

    console.log("intercept csrf 04", {request});
    return next.handle(request);
  }

  private findByActionName(name: string, actions: string[]): string | undefined {
    return actions.find(action => action.toLocaleLowerCase() === name.toLocaleLowerCase());
  }
}
