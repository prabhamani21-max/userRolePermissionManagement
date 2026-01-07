import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { NgbTypeaheadModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Observable, OperatorFunction, debounceTime, distinctUntilChanged, map, lastValueFrom } from 'rxjs';
import { UserModel } from '../../core/models/user.model';
import { ScreenDto } from '../../core/models/screen.model';
import { ScreenActionDto } from '../../core/models/screen-action.model';
import { UserPermissionOverride } from '../../core/models/user-permission-override.model';
import { Pagination } from '../../core/models/pagination.model';
import { UserService } from '../../core/services/user.service';
import { ScreenService } from '../../core/services/screen.service';
import { UpoService } from '../../core/services/upo.service';
import { RoleService } from '../../core/services/role.service';
import { PermissionService } from '../../core/services/permission.service';
import { RolePermissionDto } from '../../core/models/role-permission.model';

interface ScreenWithActions {
  screen: ScreenDto;
  actions: ScreenActionDto[];
  selectedActions: number[];
}

@Component({
  selector: 'app-userpermissionmanagement',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgbTypeaheadModule],
  templateUrl: './userpermissionmanagement.component.html',
  styleUrl: './userpermissionmanagement.component.scss',
})
export class UserPermissionManagementComponent implements OnInit {
  users: UserModel[] = [];
  screensWithActions: ScreenWithActions[] = [];
  selectedUser: UserModel | null = null;
  userRole: string = '';
  loading = false;
  saving = false;

  userForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private screenService: ScreenService,
    private upoService: UpoService,
    private roleService: RoleService,
    private permissionService: PermissionService,
    private toastr: ToastrService
  ) {
    this.userForm = this.fb.group({
      user: [null]
    });
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadScreensAndActions();
  }

  loadUsers() {
    this.userService.getAllUsers(undefined, undefined, undefined, 1, 1000).subscribe({
      next: (response: Pagination<UserModel>) => {
        this.users = response.items;
      },
      error: (err: any) => {
        this.toastr.error('Failed to load users');
        console.error(err);
      }
    });
  }

  loadScreensAndActions() {
    this.screenService.getAllScreens().subscribe({
      next: (screens: ScreenDto[]) => {
        screens.forEach((screen: ScreenDto) => {
          this.screenService.getAllScreenActions(undefined, screen.id).subscribe({
            next: (actions: ScreenActionDto[]) => {
              this.screensWithActions.push({
                screen,
                actions,
                selectedActions: []
              });
            },
            error: (err: any) => {
              console.error('Failed to load actions for screen', screen.id, err);
            }
          });
        });
      },
      error: (err: any) => {
        this.toastr.error('Failed to load screens');
        console.error(err);
      }
    });
  }

  searchUser: OperatorFunction<string, readonly UserModel[]> = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map(term => term.length < 2 ? []
        : this.users.filter(user => user.name.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
    );

  formatter = (user: UserModel) => user.name;

  onUserSelect(event: any) {
    this.selectedUser = event.item;
    if (this.selectedUser) {
      this.loadUserRole();
      this.loadPermissionsForUser(this.selectedUser.id);
    } else {
      this.resetSelections();
    }
  }

  loadUserRole() {
    if (!this.selectedUser) return;
    // Assuming user has roleId, get role name
    this.roleService.getRoleById(this.selectedUser.defaultRoleId).subscribe({
      next: (role: { name: string }) => {
        this.userRole = role.name;
      },
      error: (err: any) => {
        console.error('Failed to load role', err);
        this.userRole = '';
      }
    });
  }

  loadPermissionsForUser(userId: number) {
    this.loading = true;
    this.upoService.getUserEffectivePermissions(userId).subscribe({
      next: (actionIds: number[]) => {
        this.screensWithActions.forEach(screenWithActions => {
          screenWithActions.selectedActions = screenWithActions.actions
            .filter(a => actionIds.includes(a.id))
            .map(a => a.id);
        });
        this.loading = false;
      },
      error: (err: any) => {
        this.toastr.error('Failed to load permissions');
        console.error(err);
        this.loading = false;
      }
    });
  }

  resetSelections() {
    this.screensWithActions.forEach(s => s.selectedActions = []);
    this.userRole = '';
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
    if (!this.selectedUser) {
      this.toastr.warning('Please select a user');
      return;
    }
    this.saving = true;
    // Load role permissions
    this.permissionService.getAllRolePermissions(this.selectedUser.defaultRoleId, undefined, undefined, 1, 1000).subscribe({
      next: (response: any) => {
        const rolePermissions = response.items as RolePermissionDto[];
        const roleActionIds = rolePermissions.map(rp => rp.actionId);
        const desiredActionIds: number[] = [];
        this.screensWithActions.forEach(s => desiredActionIds.push(...s.selectedActions));

        const toAllow = desiredActionIds.filter(id => !roleActionIds.includes(id));
        const toDeny = roleActionIds.filter(id => !desiredActionIds.includes(id));

        // Load current overrides
        this.upoService.getAllUserPermissionOverrides(undefined, 1, 1000).subscribe({
          next: (overrideResponse: Pagination<UserPermissionOverride>) => {
            const currentOverrides = overrideResponse.items.filter((o: UserPermissionOverride) => o.userId === this.selectedUser!.id);

            const removePromises = currentOverrides.map((o: UserPermissionOverride) => lastValueFrom(this.upoService.deleteUserPermissionOverride(o.id)));
            const addAllowPromises = toAllow.map((actionId: number) => {
              const dto: UserPermissionOverride = {
                id: 0,
                userId: this.selectedUser!.id,
                actionId: actionId,
                isDenied: false,
                statusId: 1
              };
              return lastValueFrom(this.upoService.createUserPermissionOverride(dto));
            });
            const addDenyPromises = toDeny.map((actionId: number) => {
              const dto: UserPermissionOverride = {
                id: 0,
                userId: this.selectedUser!.id,
                actionId: actionId,
                isDenied: true,
                statusId: 1
              };
              return lastValueFrom(this.upoService.createUserPermissionOverride(dto));
            });

            Promise.all([...removePromises, ...addAllowPromises, ...addDenyPromises]).then(() => {
              this.toastr.success('Permissions updated successfully');
              this.saving = false;
            }).catch((err: any) => {
              this.toastr.error('Failed to update permissions');
              console.error(err);
              this.saving = false;
            });
          },
          error: (err: any) => {
            this.toastr.error('Failed to load current overrides');
            console.error(err);
            this.saving = false;
          }
        });
      },
      error: (err: any) => {
        this.toastr.error('Failed to load role permissions');
        console.error(err);
        this.saving = false;
      }
    });
  }
}