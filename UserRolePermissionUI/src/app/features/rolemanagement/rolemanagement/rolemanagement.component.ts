import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RoleTableComponent } from '../roletable/roletable.component';
import { Role } from '../../../core/models/role.model';
import { Observable, take } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from '../../../shared/components/confirm-dialog/confirm-dialog.service';
import { RoleService } from '../../../core/services/role.service';

@Component({
  selector: 'app-rolemanagement',
  standalone: true,
  imports: [CommonModule, RoleTableComponent],
  templateUrl: './rolemanagement.component.html',
  styleUrl: './rolemanagement.component.scss',
})
export class RoleManagementComponent {
  roles$!: Observable<Role[]>;
  loading = false;
  message: string = '';
  isSuccess: boolean = false;

  constructor(
    private router: Router,
    private toastr: ToastrService,
    private confirmationService: ConfirmationService,
    private roleService: RoleService,
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.roles$ = this.roleService.getAllRoles();
  }

  // 🔍 View role detail
  onView(role: Role): void {
    this.router.navigate([`/userRolePermission/rolemanagement/view/${role.id}`]);
  }

  // 📝 Edit role
  onEdit(role: Role): void {
    this.router.navigate([`/userRolePermission/rolemanagement/edit/${role.id}`], {
      state: { role },
    });
  }

  // ➕ Create role
  onCreate(): void {
    this.router.navigate([`/userRolePermission/rolemanagement/add`]);
  }

  // 🗑️ Delete role
  onDelete(id: number): void {
    this.confirmationService
      .confirm('Are you sure?', 'You cannot undo this action.')
      .then((confirmed) => {
        if (confirmed) {
          this.roleService
            .deleteRole(id)
            .pipe(take(1))
            .subscribe({
              next: (response) => {
                this.message =
                  response?.message || 'Role deleted successfully';
                this.isSuccess = true;
                this.toastr.success(this.message);
                this.roles$ = this.roleService.getAllRoles();
              },
              error: (err) => {
                this.message =
                  err.error?.message ||
                  err.message ||
                  'Failed to delete role';
                this.isSuccess = false;
                this.toastr.error(this.message);
                console.error('Delete error:', err);
              },
            });
        }
      });
  }
}