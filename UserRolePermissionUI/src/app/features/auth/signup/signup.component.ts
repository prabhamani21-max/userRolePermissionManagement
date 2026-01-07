import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ReactiveFormsModule,
  ValidationErrors,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { Role } from '../../../core/models/role.model';
import { CustomValidators } from '../../../common/validators/custom.validators';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss',
})
export class SignupComponent implements OnInit {
  registerForm: FormGroup;
  roles: Role[] = [];
  errorMessage: string = '';
  successMessage: string = '';
  isLoading: boolean = false;
  checkingEmail: boolean = false;
  checkingContact: boolean = false;
  private destroy$ = new Subject<void>();
  profileImageBase64: string | null = null;
  selectedFileName: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthenticationService,
    private router: Router,
  ) {
    this.registerForm = this.fb.group(
      {
        name: ['', Validators.required],
        email: [
          '',
          [Validators.required, Validators.email],
        ],
        contactNo: [
          '',
          [Validators.required, Validators.pattern(/^[0-9]{10,15}$/)],
        ],
        password: [
          '',
          [Validators.required, CustomValidators.passwordValidator()],
        ],
        confirmPassword: ['', Validators.required],
        terms: [false, Validators.requiredTrue],
      },
      { validators: CustomValidators.passwordMatchValidator },
    );
  }

  ngOnInit(): void {}
  ngOnDestroy(): void {
    this.destroy$.next(); // Emit signal to unsubscribe
    this.destroy$.complete(); // Complete the subject
  }

  // Updated image validation (optional)
  validateImage(file: File): ValidationErrors | null {
    if (!file) return null; // Image is optional

    // Check file type
    const validTypes = ['image/jpeg', 'image/png'];
    if (!validTypes.includes(file.type)) {
      return { invalidType: true };
    }

    // Check file size (2MB max, 10KB min)
    if (file.size > 2 * 1024 * 1024) {
      return { maxSize: true };
    }

    if (file.size < 10 * 1024) {
      return { minSize: true };
    }

    return null;
  }
  // Update onFileSelected method to properly handle validation
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];

      // Validate the file
      const validationResult = this.validateImage(file);

      if (validationResult) {
        // Set errors and reset the file selection
        this.registerForm.get('profileImage')?.setErrors(validationResult);
        this.selectedFileName = null;
        this.profileImageBase64 = null;
        input.value = ''; // Clear the file input
      } else {
        // File is valid, proceed with processing
        this.selectedFileName = file.name;

        this.convertToBase64(file)
          .then((base64) => {
            this.profileImageBase64 = base64;
            this.registerForm.patchValue({
              profileImage: base64,
            });
            this.registerForm.get('profileImage')?.setErrors(null); // Clear any previous errors
          })
          .catch((error) => {
            console.error('Error converting image:', error);
            this.registerForm
              .get('profileImage')
              ?.setErrors({ conversionError: true });
            this.selectedFileName = null;
            this.profileImageBase64 = null;
            input.value = ''; // Clear the file input
          });
      }
    }
  }

  private convertToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve((reader.result as string).split(',')[1]);
      reader.onerror = (error) => reject(error);
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.registerForm.value;
    const user = {
      name: formValue.name,
      email: formValue.email,
      password: formValue.password,
      defaultRoleId: formValue.defaultRoleId,
      gender: formValue.gender,
      dob: formValue.dob,
      contactNo: formValue.contactNo,
      profileImage: formValue.profileImage || '', // Send null if no image
    };

    this.authService.register(user).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.Status) {
          this.successMessage =
            'Registration successful! Redirecting to login...';
          setTimeout(() => {
            this.router.navigate(['/userRolePermission/auth/sign-in'], {
              queryParams: { registered: 'true' },
            });
          }, 2000);
        } else {
          this.errorMessage =
            response.Message || 'Registration failed. Please try again.';
        }
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Registration error:', error);
        this.errorMessage =
          error.error?.Message ||
          'An error occurred during registration. Please try again.';
      },
    });
  }
}
