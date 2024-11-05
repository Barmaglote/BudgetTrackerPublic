import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function categoryValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const category = control.value;

    if (!category || Object.keys(category).length === 0 || !category['category']) {
      return { 'requiredCategory': true };
    }

    return null;
  };
}
