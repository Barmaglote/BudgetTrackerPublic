import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { Observable, throwError, switchMap, of, BehaviorSubject, ObservableInput } from 'rxjs';
import { catchError, filter, take } from 'rxjs/operators';
import { HashMap, TranslocoService, translate } from '@ngneat/transloco';
import { AuthenticationService } from '../services';
import { AppError } from '../models';
import { LoginService } from 'src/app/modules/login/services';

@Injectable({ providedIn: 'root' })
export class HttpErrorInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  private TOKEN_HEADER_KEY = 'x-access-token';

  constructor(
    private messageService: MessageService,
    private router: Router,
    private translateService: TranslocoService,
    private authenticationService: AuthenticationService,
    private loginService: LoginService
  ) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((err) => {
        console.log("intercept http-error", {request, err});

        if (err.error instanceof Blob) {
          return new Promise<any>((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = (e: Event) => {
              try {
                const errmsg = JSON.parse((e.target as any).result);
                err.error = errmsg;
                this.getError(err);
              } catch (e) {
                reject(err);
              }
            };
            reader.onerror = (e) => {
              reject(err);
            };
            reader.readAsText(err.error);
          });
        } else if (err instanceof HttpErrorResponse && !request.url.includes('/api/auth/signin') && err.status === 401) {
            return this.handle401Error(request, next);
        } else {
            this.getError(err);
            return throwError(() => err);
        }
      }),
    );
  }

  private getError(err: any) {
    switch (err.status) {
      case 0: {
        try {
          if (err.url.includes('/refreshtoken')) {
            break;
          }
        } catch {}

        throw new AppError(translate('exceptions.API_NOT_AVAILABLE'), true, {
          severity: 'error', life: 1000
        });
      }
      case 401: {
        // auto logout if 401 response returned from api
        if (!this.isRefreshing) {
          this.authenticationService.logout();
          this.authenticationService.toLogin(this.router.url);
        }
        break;
      }
      case 403: {
        this.router.navigate(['/forbidden']);
        throw new AppError(this.getErrorMessage(err) + ' No rights to see the Site', true, {
          severity: 'error', life: 1000
        });
      }
      default: {
        console.log("default", {err});
        throw new AppError(translate(this.getErrorMessage(err), err?.error?.args), true, {
          severity: 'error', life: 1000
        });
      }
    }
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const token = this.authenticationService.getRefreshToken();

      if (token)
        return this.loginService.refreshtoken(token).pipe(
          switchMap((data: any) => {
            this.isRefreshing = false;
              if (data?.status === 'success') {

              this.authenticationService.saveUser({
                provider: 'budgettracker',
                id: '000',
                email: data.user.login,
                name: data.user.username,
                photoUrl: '',
                firstName: '',
                lastName: '',
                authToken: data.accessToken,
                idToken: data.accessToken,
                authorizationCode: data.refreshToken,
                response: ''
              });

              this.authenticationService.saveToken(data.accessToken);
              this.authenticationService.saveRefreshToken(data.refreshToken);
              this.refreshTokenSubject.next(data.accessToken);
            }
            return next.handle(this.addTokenHeader(request, data.accessToken));
          }),
          catchError((err) => {
            this.isRefreshing = false;
            this.authenticationService.signOut();
            return throwError(() => err);
          })
        );
    }

    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request, token)))
    );
  }

  private addTokenHeader(request: HttpRequest<any>, token: string) {
    return request.clone({ headers: request.headers.set(this.TOKEN_HEADER_KEY, token) });
  }

  private getErrorMessage(err: any) {
    let error = err.statusText;
    if (err?.error !== null) {
      if (err.error?.message) {
        error = err.error?.message;
      } else {
        if (err.error?.error) {
          if (err.error?.error?.message) {
            error = err.error.error.message;
          } else {
            error = err.error.error;
          }
        }
      }
    } else if (err.message) {
      error = err.message;
    }
    return error;
  }


  private localizeMessage(message: string, translationsParams: HashMap | HashMap[] | undefined): Observable<string> {
    const tParams = translationsParams ? (Array.isArray(translationsParams) ? translationsParams[0] : translationsParams) : undefined;
    const translationKey = `exceptions.${this.replaceAll(message, ' ', '_')}`;

    return this.translateService.selectTranslate(translationKey, tParams).pipe(
      switchMap(translated => {
        if (translated !== message && !translated.startsWith('exceptions.')) {
          return of(translated);
        } else {
          return of(message);
        }
      })
    );
  }

  private openSnackbar(message: string, severity: string, vibrate: boolean) {
    this.messageService.clear();
    this.messageService.add({ severity, summary: '', detail: message, sticky: false, life: 1000 });
    if (vibrate) {
      window.navigator.vibrate(500);
    }
  }

  private replaceAll(str: string, find: string, replace: string) {
    if (!str) {
      return '';
    }
    return str.replace(new RegExp(this.escapeRegExp(find), 'g'), replace);
  }

  private escapeRegExp(msg: string) {
    if (!msg) {
      return '';
    }
    return msg.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
  }
}
