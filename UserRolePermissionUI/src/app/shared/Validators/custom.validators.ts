import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
  AsyncValidatorFn,
} from '@angular/forms';
import { AuthenticationService } from '../../core/services/auth.service';
import { timer, of, Observable } from 'rxjs';
import { map, switchMap, catchError } from 'rxjs/operators';

export class CustomValidators {
  static passwordMatchValidator: ValidatorFn = (
    control: AbstractControl,
  ): ValidationErrors | null => {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (password?.value !== confirmPassword?.value) {
      return { mismatch: true };
    }
    return null;
  };

  static emailAvailabilityValidator(
    authService: AuthenticationService,
    currentUserId: number | null = null,
    originalEmail: string | null = null,
  ): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value || control.errors?.['email']) {
        return of(null);
      }
      if (originalEmail && control.value === originalEmail) {
        return of(null);
      }
      return timer(500).pipe(
        switchMap(() =>
          authService.checkEmailExists(control.value, currentUserId),
        ),
        map((response) => {
          const conflict = response.message?.toLowerCase().includes('already exists');
          return conflict ? { emailExists: true } : null;
        }),
        catchError(() => of(null)),
      );
    };
  }

  static contactAvailabilityValidator(
    authService: AuthenticationService,
    currentUserId: number | null = null,
    originalContact: string | null = null,
  ): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value || control.errors?.['pattern']) {
        return of(null);
      }
      if (originalContact && control.value === originalContact) {
        return of(null);
      }
      return timer(500).pipe(
        switchMap(() =>
          authService.checkContactExists(control.value, currentUserId),
        ),
        map((response) => {
          const conflict = response.message?.toLowerCase().includes('already exists');
          return conflict ? { contactExists: true } : null;
        }),
        catchError(() => of(null)),
      );
    };
  }

  static passwordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;

      const strongRegex = /^(?=.*[A-Z])(?=.*[!@#$%^&*(),.?":{}|<>]).{9,}$/;
      return strongRegex.test(value) ? null : { passwordStrength: true };
    };
  }
}
