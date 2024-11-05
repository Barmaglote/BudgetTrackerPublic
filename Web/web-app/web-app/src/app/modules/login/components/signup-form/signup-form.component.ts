import { Component, EventEmitter, Output, forwardRef } from '@angular/core';
import { FormBuilder, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { AppError } from 'src/app/core/models';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-signup-form',
  templateUrl: './signup-form.component.html',
  styleUrls: ['./signup-form.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SignupFormComponent),
      multi: true,
    },
  ],
})
export class SignupFormComponent {
  public recaptcha: string = '';
  protected siteKey = environment.recaptchakey;
  public signupForm: FormGroup = this.formBuilder.group({
    title: [null, [Validators.required, Validators.maxLength(30)]],
    email: [null, [Validators.required, Validators.email]],
    password: [null, [Validators.required, Validators.minLength(8), Validators.pattern(/[!@#$%^&*(),.?":{}|<>]/)]],
    password2: [null, [Validators.required, Validators.minLength(8), Validators.pattern(/[!@#$%^&*(),.?":{}|<>]/)]],
    recaptcha: ['', Validators.required],
  });

  @Output() submit = new EventEmitter<{title: string, email: string, password: string, recaptcha: string}>();

  constructor(private formBuilder: FormBuilder) { }

  onSubmit(){
    let { title, email, password, password2, recaptcha } = this.signupForm.value;

    if (password !== password2) {
      throw new AppError('Password are not equal', true, {
        severity: 'error', life: 1000
      });
    }

    if (this.signupForm.invalid) {
      this.signupForm.markAllAsTouched();
      return;
    }

    this.submit.emit({title, email, password, recaptcha});
  }
}
