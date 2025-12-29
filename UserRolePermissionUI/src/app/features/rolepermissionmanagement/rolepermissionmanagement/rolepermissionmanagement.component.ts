import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Role } from '../../../core/models/role.model';
import { ScreenDto } from '../../../core/models/screen.model';
import { ScreenActionDto } from '../../../core/models/screen-action.model';
import { RolePermissionDto } from '../../../core/models/role-permission.model';
import { AuthenticationService } from '../../../core/services/auth.service';
import { ScreenService } from '../../../core/services/screen.service';
import { PermissionService } from '../../../core/services/permission.service';

interface ScreenWithActions {
  screen: ScreenDto;
  actions: ScreenActionDto[];
  selectedActions: number[];
}

@Component({
  selector: 'app-rolepermissionmanagement',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './rolepermissionmanagement.component.html',
  styleUrl: './rolepermissionmanagement.component.scss',
})
export class RolePermissionManagementComponent implements OnInit {
  roles: Role[] = [];
  screensWithActions: ScreenWithActions[] = [];
  selectedRoleId: number | null = null;
  loading = false;
  saving = false;

  roleForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthenticationService,
    private screenService: ScreenService,
    private permissionService: PermissionService,
    private toastr: ToastrService
  ) {
    this.roleForm = this.fb.group({
      roleId: [null]
    });
  }

  ngOnInit(): void {
    this.loadRoles();
    this.loadScreensAndActions();
  }

  loadRoles() {
    this.authService.fetchRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
      },
      error: (err) => {
        this.toastr.error('Failed to load roles');
        console.error(err);
      }
    });
  }

  loadScreensAndActions() {
    this.screenService.getAllScreens().subscribe({
      next: (screens) => {
        screens.forEach(screen => {
          this.screenService.getAllScreenActions(undefined, screen.id).subscribe({
            next: (actions) => {
              this.screensWithActions.push({
                screen,
                actions,
                selectedActions: []
              });
            },
            error: (err) => {
              console.error('Failed to load actions for screen', screen.id, err);
            }
          });
        });
      },
      error: (err) => {
        this.toastr.error('Failed to load screens');
        console.error(err);
      }
    });
  }

  onRoleChange() {
    this.selectedRoleId = this.roleForm.value.roleId;
    if (this.selectedRoleId) {
      this.loadPermissionsForRole(this.selectedRoleId);
    } else {
      this.resetSelections();
    }
  }

  loadPermissionsForRole(roleId: number) {
    this.loading = true;
    this.permissionService.getAllRolePermissions(roleId, undefined, 1, 1, 1000).subscribe({
      next: (response) => {
        const permissions = response.items as RolePermissionDto[];
        this.screensWithActions.forEach(screenWithActions => {
          screenWithActions.selectedActions = permissions
            .filter(p => screenWithActions.actions.some(a => a.id === p.actionId))
            .map(p => p.actionId);
        });
        this.loading = false;
      },
      error: (err) => {
        this.toastr.error('Failed to load permissions');
        console.error(err);
        this.loading = false;
      }
    });
  }

  resetSelections() {
    this.screensWithActions.forEach(s => s.selectedActions = []);
  }

  onActionToggle(screenWithActions: ScreenWithActions, actionId: number) {
    const index = screenWithActions.selectedActions.indexOf(actionId);
    if (index > -1) {
      screenWithActions.selectedActions.splice(index, 1);
    } else {
      screenWithActions.selectedActions.push(actionId);
    }
  }

  savePermissions() {
    if (!this.selectedRoleId) {
      this.toastr.warning('Please select a role');
      return;
    }
    this.saving = true;
    // First, get current permissions
    this.permissionService.getAllRolePermissions(this.selectedRoleId, undefined, 1, 1, 1000).subscribe({
      next: (response) => {
        const currentPermissions = response.items as RolePermissionDto[];
        const currentActionIds = currentPermissions.map(p => p.actionId);
        const desiredActionIds: number[] = [];
        this.screensWithActions.forEach(s => desiredActionIds.push(...s.selectedActions));

        const toAdd = desiredActionIds.filter(id => !currentActionIds.includes(id));
        const toRemove = currentPermissions.filter(p => !desiredActionIds.includes(p.actionId));

        // Remove
        const removePromises = toRemove.map(p => this.permissionService.deleteRolePermission(p.id).toPromise());

        // Add
        const addPromises = toAdd.map(actionId => {
          const dto: RolePermissionDto = {
            id: 0,
            roleId: this.selectedRoleId!,
            actionId: actionId,
            statusId: 1
          };
          return this.permissionService.createRolePermission(dto).toPromise();
        });

        Promise.all([...removePromises, ...addPromises]).then(() => {
          this.toastr.success('Permissions updated successfully');
          this.saving = false;
        }).catch(err => {
          this.toastr.error('Failed to update permissions');
          console.error(err);
          this.saving = false;
        });
      },
      error: (err) => {
        this.toastr.error('Failed to load current permissions');
        console.error(err);
        this.saving = false;
      }
    });
  }
}