import { Component, EventEmitter, Output, forwardRef } from '@angular/core';
import { FormBuilder, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-restore-request-form',
  templateUrl: './restore-request-form.component.html',
  styleUrls: ['./restore-request-form.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RestoreRequestFormComponent),
      multi: true,
    },
  ],
})
export class RestoreRequestFormComponent {
  @Output() submit = new EventEmitter<{email: string, recaptcha: string}>();
  protected siteKey = environment.recaptchakey;
  public restoreForm: FormGroup = this.formBuilder.group({
    email: [null, [Validators.required]],
    recaptcha: ['', Validators.required],
  });
  constructor(private formBuilder: FormBuilder) { }
  onSubmit(){
    let { email, recaptcha } = this.restoreForm.value;
    if (this.restoreForm.invalid) {
      this.restoreForm.markAllAsTouched();
      return;
    }

    this.submit.emit({email, recaptcha});
  }
}
