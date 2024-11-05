import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-change-password-request-form',
  templateUrl: './change-password-request-form.component.html',
  styleUrls: ['./change-password-request-form.component.css']
})
export class ChangePasswordRequestFormComponent {
  @Output() submit = new EventEmitter<{oldpassword: string, newpassword: string}>();
  public changePasswordForm: FormGroup = this.formBuilder.group({
    oldpassword: [null, [Validators.required]],
    newpassword: [null, [Validators.required]],
    newpasswordconfirmation: [null, [Validators.required]],
  });

  constructor(private formBuilder: FormBuilder) { }

  onSubmit(){
    let { oldpassword, newpassword, newpasswordconfirmation } = this.changePasswordForm.value;
    if (!oldpassword || !newpassword || newpassword !== newpasswordconfirmation) {
      return;
    }
    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }

    this.submit.emit({oldpassword, newpassword});
  }
}
