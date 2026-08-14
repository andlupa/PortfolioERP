import { Injectable } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { ValidationProblemDetails } from '../models/validation-problem-details';

@Injectable({
  providedIn: 'root'
})
export class FormValidationService {
  applyBackendErrors(
    form: FormGroup,
    problem: ValidationProblemDetails | null | undefined
  ): void {
    const errors = problem?.errors;

    if (!errors) {
      return;
    }

    for (const [propertyName, messages] of Object.entries(errors)) {
      const control = this.findControl(form, propertyName);

      if (!control) {
        continue;
      }

      control.setErrors({
        ...control.errors,
        backend: messages.join(' ')
      });

      control.markAsTouched();
    }
  }

  getBackendError(control: AbstractControl): string | null {
    return control.getError('backend') ?? null;
  }

  clearBackendError(control: AbstractControl): void {
    if (!control.hasError('backend')) {
      return;
    }

    const currentErrors = {
      ...control.errors
    };

    delete currentErrors['backend'];

    control.setErrors(
      Object.keys(currentErrors).length > 0
        ? currentErrors
        : null
    );
  }

  private findControl(
    form: FormGroup,
    propertyName: string
  ): AbstractControl | null {
    const camelCaseName =
      propertyName.charAt(0).toLowerCase() +
      propertyName.slice(1);

    return form.get(camelCaseName);
  }

  clearBackendErrorsOnValueChanges(form: FormGroup): void {
    for (const control of Object.values(form.controls)) {
      control.valueChanges.subscribe(() => {
        this.clearBackendError(control);
      });
    }
  }
}
