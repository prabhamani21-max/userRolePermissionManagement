import { Component, inject } from '@angular/core';
import {
  FormsModule,
  ReactiveFormsModule,
  UntypedFormBuilder,
  Validators,
  type UntypedFormGroup,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthenticationService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { MenuService } from '../../../core/services/menu.service';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './signin.component.html',
})
export class SigninComponent {
  signInForm!: UntypedFormGroup;
  submitted: boolean = false;
  isLoading: boolean = false;
  errorMessage: string | null = null;

  private fb = inject(UntypedFormBuilder);
  private authService = inject(AuthenticationService);
  private router = inject(Router);
  private toastr = inject(ToastrService); // Inject ToastrService
  private menuService = inject(MenuService);

  ngOnInit(): void {
    this.signInForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  get formValues() {
    return this.signInForm.controls;
  }

  login() {
    this.submitted = true;
    this.errorMessage = null;

    if (this.signInForm.valid) {
      this.isLoading = true;
      const { email, password } = this.signInForm.value;

      this.authService.login(email, password).subscribe({
        next: () => {
          // Verify token is stored and decoded properly
          const token = this.authService.token;
          const decodedToken = this.authService.getDecodedToken();

          // Navigate based on role
          if (decodedToken && decodedToken.roleId) {
            const roleId = parseInt(decodedToken.roleId);
            if (roleId === 3) {
              this.router.navigate(['/userRolePermission/HR']);
            } else {
              this.router.navigate(['/userRolePermission/admin']);
            }
          } else {
            this.router.navigate(['/userRolePermission/admin']);
          }
        },
        error: (error) => {
          this.isLoading = false;

          if (error.status === 404) {
            this.toastr.error('Invalid email or password.');
          } else {
            this.toastr.error('Login failed. Please try again.');
          }
        },
      });
    }
  }
}
