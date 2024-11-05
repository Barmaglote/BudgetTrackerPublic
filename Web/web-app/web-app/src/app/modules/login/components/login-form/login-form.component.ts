import { Component, EventEmitter, Input, OnInit, Output, forwardRef } from '@angular/core';
import { FormBuilder, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { LOGIN_ROUTE, LOGIN_ROUTE_MAP } from '../../login-routing.module';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => LoginFormComponent),
      multi: true,
    },
  ],
})
export class LoginFormComponent implements OnInit {
  @Input() email: string | undefined | null;
  @Input() password: string | undefined | null;
  protected siteKey = environment.recaptchakey;
  protected env = environment.env;
  public restoreLink: string = `/${LOGIN_ROUTE}/${LOGIN_ROUTE_MAP.restoreRequest}`;
  public isNotARobot: boolean = false;
  public recaptcha: string = '';

  @Output() submit = new EventEmitter<{email: string, password: string, recaptcha: string, rememberme: boolean}>();
  public loginForm: FormGroup = this.env === 'test' ? this.formBuilder.group({
    email: [null, [Validators.required]],
    password: [null, [Validators.required]],
    recaptcha: [''],
    rememberme: [null],
  }) : this.formBuilder.group({
    email: [null, [Validators.required]],
    password: [null, [Validators.required]],
    recaptcha: ['', Validators.required],
    rememberme: [null],
  })

  constructor(private formBuilder: FormBuilder) {
  }
  ngOnInit(): void {
    if (this.email && this.password) {
      this.loginForm.setValue({email: this.email, password: this.password, rememberme: false, recaptcha: ''});
    }

  }

  onSubmit(){
   let { email, password, recaptcha, rememberme } = this.loginForm.value;
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.submit.emit({email, password, recaptcha, rememberme});
  }

  resolved(captchaResponse: string) {
    this.recaptcha = captchaResponse;
  }
}
