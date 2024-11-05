import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-restore-form',
  templateUrl: './restore-form.component.html',
  styleUrls: ['./restore-form.component.css']
})
export class RestoreFormComponent {
  @Output() submit = new EventEmitter<{password: string}>();
  public restoreForm: FormGroup = this.formBuilder.group({
    newpassword: [null, [Validators.required]],
    newpasswordconfirmation: [null, [Validators.required]],
  });
  constructor(private formBuilder: FormBuilder) { }
  onSubmit(){
    let { newpassword, newpasswordconfirmation } = this.restoreForm.value;
    if (this.restoreForm.invalid || newpassword !== newpasswordconfirmation) {
      this.restoreForm.markAllAsTouched();
      return;
    }

    this.submit.emit({password: newpassword});
  }

}
