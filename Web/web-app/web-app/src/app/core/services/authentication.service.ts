import { SocialAuthService, SocialUser } from "@abacritt/angularx-social-login";
import { Injectable, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { translate } from "@ngneat/transloco";
import { Store } from "@ngrx/store";
import { MessageService } from "primeng/api";
import { BehaviorSubject, Observable, Subscription, catchError, firstValueFrom, tap, throwError } from "rxjs";
import * as settingsFeatureStore from '../../modules/settings/store';
import * as creditsFeatureStore from '../../modules/credits/store';
import * as mainFeatureStore from '../../modules/main/store';
import * as planningFeatureStore from '../../modules/planning/store';
import { HttpClient } from "@angular/common/http";
import { environment } from 'src/environments/environment';
import { GarbageCollector } from "src/app/models";

const TOKEN_KEY = 'auth-token';
const REFRESHTOKEN_KEY = 'auth-refreshtoken';
const USER_KEY = 'auth-user';

interface CSRFResponse {
  csrfToken: string
}

@Injectable({ providedIn: 'root' })
export class AuthenticationService implements OnInit, OnDestroy {

  private userSubject$: BehaviorSubject<SocialUser | undefined>;
  private _initStateSubscription: Subscription | undefined;
  private _authStateSubscription: Subscription | undefined;

  private controllerAPIUrl: string;
  private returnUrl: string | undefined = undefined;

  constructor(
    private authService: SocialAuthService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private store: Store,
    private http: HttpClient
  ) {
    this.controllerAPIUrl = `${environment.webapi}`;
    this.userSubject$ = new BehaviorSubject<SocialUser | undefined>(this.getUser());

    this._initStateSubscription = this.authService.initState.subscribe((isReady) => {
      console.log(isReady ? 'all providers are ready' : 'provider are not ready');
    });

    this._authStateSubscription = this.authService.authState.subscribe((data) => {
      this.signUser(data);
      this.updateCSRF().subscribe(() => {
        this.redirect();
      });
    });
  }
  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([this.userSubject$]);
    garbageCollector.collect();

    if (this._initStateSubscription) {
      this._initStateSubscription.unsubscribe();
    }

    if (this._authStateSubscription) {
      this._authStateSubscription.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params: any) => {
      this.returnUrl = params['returnUrl'];
    });
  }

  private redirect() {
    if (this.returnUrl) {
      if (decodeURIComponent(this.returnUrl).startsWith('/user/login')) {
        this.router.navigate([decodeURIComponent(this.returnUrl)]);
      } else {
        this.router.navigate(['/user/accounts']);
      }
    } else {
      this.router.navigate(['/user/accounts']);
    }
  }

  public signUser(data: any) {
    this.saveUser(data);

    if (data) {
      const welcome = translate('authentication.welcome', {user: data?.name});
      this.messageService.clear();
      this.messageService.add({
        severity: 'success',
        summary: 'Authentication',
        detail: welcome });
    }

    this.saveRefreshToken(data?.authorizationCode);
    this.saveToken(data?.authToken);
  }

  public isAuthenticated() {
    return this.getUser().id !== undefined;
  }

  public logout(){
    this.router.navigate(['/login']);
    window.sessionStorage.clear();
    this.cleanStores();
  }

  public signOut(): void {
    window.sessionStorage.clear();
    this.cleanStores();
  }

  public signIn(id: string){
    this.authService.signIn(id);
  }

  public getUserObject() {
    return this.userSubject$;
  }

  public saveUser(value: SocialUser | undefined): void {
    window.sessionStorage.removeItem(USER_KEY);
    window.sessionStorage.setItem(USER_KEY, JSON.stringify(value));
  }

  toLogin(returnUrl: string) {
    this.router.navigate(['/login'], { queryParams: { returnUrl } });
  }

  private cleanStores() {
    this.store.dispatch(settingsFeatureStore.SettingsActions.cleanStore());
    this.store.dispatch(creditsFeatureStore.CreditsActions.cleanStore());
    this.store.dispatch(mainFeatureStore.MainActions.cleanStore());
    this.store.dispatch(planningFeatureStore.PlanningActions.cleanStore());
  }

  updateCSRF(): Observable<any> {
    return this.http.get<CSRFResponse>(`${this.controllerAPIUrl}/session/csrf`, { withCredentials: true }).pipe(
      tap((response) => {
        document.cookie = `XSRF-TOKEN=${response?.csrfToken}; Secure; Path=/`;
      }),
      catchError((error) => {
        console.error("Error fetching CSRF token:", error);
        return throwError(error);
      })
    );
  }

  public saveRefreshToken(token: string): void {
    window.sessionStorage.removeItem(REFRESHTOKEN_KEY);
    window.sessionStorage.setItem(REFRESHTOKEN_KEY, token);
  }

  public getRefreshToken(): string | null {
    return window.sessionStorage.getItem(REFRESHTOKEN_KEY);
  }

  public getUser(): any {
    const user = window.sessionStorage.getItem(USER_KEY);
    if (user) {
      return JSON.parse(user);
    }

    return {};
  }

  public saveToken(token: string): void {
    window.sessionStorage.removeItem(TOKEN_KEY);
    window.sessionStorage.setItem(TOKEN_KEY, token);

    const user = this.getUser();
    if (user.id) {
      this.saveUser({ ...user, accessToken: token });
    }
  }
}
