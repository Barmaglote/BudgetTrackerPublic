import { FacebookLoginProvider } from '@abacritt/angularx-social-login';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { Product } from '../../models/products';
import { ProductService } from '../../services';
import { Store } from '@ngrx/store';
import * as featureStore from '../../store';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from '../../login-routing.module';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  products: Product[] = [];
  responsiveOptions: any[] | undefined;
  returnUrl: string | undefined = undefined;
  public signUpUrl: string = `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.signup}`;
  public email: string | undefined | null = undefined;
  public password: string | undefined | null = undefined;
  public environment = environment;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private productService: ProductService,
    private authenticationService: AuthenticationService,
    private store: Store
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe((params: any) => {
      this.returnUrl = params['returnUrl'];
    }).unsubscribe();

    this.productService.getProducts().then((products) => {
        this.products = products;
    });

    this.responsiveOptions = [
        {
            breakpoint: '1199px',
            numVisible: 1,
            numScroll: 1
        },
        {
            breakpoint: '991px',
            numVisible: 2,
            numScroll: 1
        },
        {
            breakpoint: '767px',
            numVisible: 1,
            numScroll: 1
        }
    ];

    // TODO: Проверить, как работает авторизация с Google. Получается, что два раза вызывается переход
    // if (this.authenticationService.isAuthenticated()) {
    //   this.router.navigate(['/user/accounts']);
    // }

    this.restoreme();
  }

  signInWithFB(): void {
    this.authenticationService.signIn(FacebookLoginProvider.PROVIDER_ID);
  }

  onSubmit(event: any) {
    let { email, password, recaptcha, rememberme } = event;
    console.log("onSubmit", environment.env);
    if (!email || !password || (environment.env !== 'test' && !recaptcha)) { return; }

    if (rememberme === true) {
      this.rememberme(email, password);
    }

    this.store.dispatch(featureStore.LoginActions.signIn(email, password, recaptcha));
  }

  private rememberme(email: string, password: string) {
    localStorage.setItem('email', email);
    localStorage.setItem('password', password);
  }

  private restoreme() {
    this.email = localStorage.getItem('email');
    this.password = localStorage.getItem('password');
  }
}
