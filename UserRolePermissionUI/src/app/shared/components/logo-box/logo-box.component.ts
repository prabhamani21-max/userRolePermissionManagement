import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { AuthenticationService } from '../../../core/services/auth.service';
import { RoleEnum } from '../../../core/enums/role.enum';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-logo-box',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div [class]="className">
      <a [routerLink]="dashboardRoute" class="logo-dark">
        <img
          src="assets/images/logo-sm.png"
          [ngClass]="className == 'logo-box' ? 'logo-sm' : 'me-1'"
          [height]="logoHeight"
          alt="logo sm"
        />
      </a>

      <a [routerLink]="dashboardRoute" class="logo-light">
        <img
          src="assets/images/logo-sm.png"
          [ngClass]="className == 'logo-box' ? 'logo-sm' : 'me-1'"
          [height]="logoHeight"
          alt="logo sm"
        />
      </a>
    </div>
  `,
})
export class LogoBoxComponent {
  @Input() className: string = '';
  @Input() height: string = '';
  @Input() logoHeight: string = '';
  private authService = inject(AuthenticationService);
  private router = inject(Router);

  get dashboardRoute(): string {
    const decodedToken = this.authService.getUserInformation();
    const roleId = decodedToken?.roleId;

    switch (roleId) {
      case RoleEnum.SuperAdmin.toString():
      case RoleEnum.Admin.toString():
        return '/userRolePermission/dashboard';
      case RoleEnum.Applicant.toString():
        return '/userRolePermission/dashboard/applicant';
      case RoleEnum.Client.toString():
        return '/userRolePermission/dashboard/client';
      case RoleEnum.HR.toString():
        return '/userRolePermission/dashboard/hr';
      case RoleEnum.SalesCoOrdinator.toString():
        return '/userRolePermission/dashboard/sales';
      case RoleEnum.BDE.toString():
        return '/userRolePermission/dashboard/bde';
      case RoleEnum.BDM.toString():
        return '/userRolePermission/dashboard/bdm';
      default:
        return '/userRolePermission/auth/sign-in'; // Fallback for unauthorized or missing role
    }
  }
}
