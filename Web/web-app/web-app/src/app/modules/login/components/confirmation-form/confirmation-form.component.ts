import { Component, EventEmitter, OnInit, Output, forwardRef } from '@angular/core';
import { FormBuilder, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';

@Component({
  selector: 'app-confirmation-form',
  templateUrl: './confirmation-form.component.html',
  styleUrls: ['./confirmation-form.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ConfirmationFormComponent),
      multi: true,
    },
  ],
})
export class ConfirmationFormComponent {
  @Output() submit = new EventEmitter<{email: string}>();
  public confirmForm: FormGroup = this.formBuilder.group({
    email: [null, [Validators.required]],
  });

  constructor(private formBuilder: FormBuilder) { }

  onSubmit(){
    let { email } = this.confirmForm.value;

    if (this.confirmForm.invalid) {
      this.confirmForm.markAllAsTouched();
      return;
    }

    this.submit.emit({email});
  }
}
