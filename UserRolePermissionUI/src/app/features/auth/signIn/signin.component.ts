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
import { MenuItem } from '../../../common/menu-meta';

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
    console.log('SigninComponent ngOnInit called');
    this.signInForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  get formValues() {
    return this.signInForm.controls;
  }

  private findDefaultDashboard(menuItems: MenuItem[]): MenuItem | null {
    for (const item of menuItems) {
      if (item.isDefaultDashboard) {
        return item;
      }
      if (item.subMenu) {
        const found = this.findDefaultDashboard(item.subMenu);
        if (found) return found;
      }
    }
    return null;
  }

  login() {
    this.submitted = true;
    this.errorMessage = null;

    if (this.signInForm.valid) {
      this.isLoading = true;
      const { email, password } = this.signInForm.value;

      this.authService.login(email, password).subscribe({
        next: () => {
          // Fetch sidebar menu to determine default dashboard
          this.menuService.getSidebarMenu().subscribe({
            next: (menuItems) => {
              const defaultDashboard = this.findDefaultDashboard(menuItems);
              if (defaultDashboard && defaultDashboard.link) {
                this.router.navigate([defaultDashboard.link]);
              // } else {
              //   // Fallback to old logic if no default dashboard found
              //   const decodedToken = this.authService.getDecodedToken();
              //   if (decodedToken && decodedToken.roleId) {
              //     const roleId = parseInt(decodedToken.roleId);
              //     if (roleId === 3) {
              //       this.router.navigate(['/userRolePermission/HR']);
              //     } else {
              //       this.router.navigate(['/userRolePermission/admin']);
              //     }
              //   } else {
              //     this.router.navigate(['/userRolePermission/admin']);
              //   }
              }
            },
            error: (error) => {
              console.error('Failed to load sidebar menu for navigation', error);
              // Fallback to old logic on error
              const decodedToken = this.authService.getDecodedToken();
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
            }
          });
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
