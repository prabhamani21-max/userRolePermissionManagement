import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbModal, NgbModalModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { UserRole } from '../../core/models/user-role.model';
import { Pagination } from '../../core/models/pagination.model';
import { UserModel } from '../../core/models/user.model';
import { Role } from '../../core/models/role.model';
import { UserRoleService } from '../../core/services/user-role.service';
import { UserService } from '../../core/services/user.service';
import { RoleService } from '../../core/services/role.service';

@Component({
  selector: 'app-userrolemanagement',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NgbModalModule, NgbPaginationModule],
  templateUrl: './userrolemanagement.component.html',
  styleUrl: './userrolemanagement.component.scss',
})
export class UserRoleManagementComponent implements OnInit {
  userRoles: UserRole[] = [];
  users: UserModel[] = [];
  roles: Role[] = [];
  pagination: Pagination<UserRole> = {
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 0,
  };
  loading = false;
  currentPage = 1;
  pageSize = 10;

  userRoleForm: FormGroup;
  editMode = false;
  currentUserRoleId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private userRoleService: UserRoleService,
    private userService: UserService,
    private roleService: RoleService,
    private modalService: NgbModal,
    private toastr: ToastrService
  ) {
    this.userRoleForm = this.fb.group({
      userId: ['', Validators.required],
      roleId: ['', Validators.required],
      statusId: [1, Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
    this.loadUserRoles();
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

  loadRoles() {
    this.roleService.getAllRoles().subscribe({
      next: (roles: Role[]) => {
        this.roles = roles;
      },
      error: (err: any) => {
        this.toastr.error('Failed to load roles');
        console.error(err);
      }
    });
  }

  loadUserRoles() {
    this.loading = true;
    this.userRoleService.getAllUserRoles(undefined, undefined, undefined, this.currentPage, this.pageSize).subscribe({
      next: (response: Pagination<UserRole>) => {
        this.pagination = response;
        this.userRoles = response.items;
        this.loading = false;
      },
      error: (err: any) => {
        this.toastr.error('Failed to load user roles');
        console.error(err);
        this.loading = false;
      }
    });
  }

  onPageChange(page: number) {
    this.currentPage = page;
    this.loadUserRoles();
  }

  openAddModal(content: any) {
    this.editMode = false;
    this.userRoleForm.reset({ statusId: 1 });
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  openEditModal(content: any, userRole: UserRole) {
    this.editMode = true;
    this.currentUserRoleId = userRole.id;
    this.userRoleForm.patchValue({
      userId: userRole.userId,
      roleId: userRole.roleId,
      statusId: userRole.statusId,
    });
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  saveUserRole() {
    if (this.userRoleForm.invalid) {
      this.toastr.warning('Please fill all required fields');
      return;
    }

    const formValue = this.userRoleForm.value;
    const dto: UserRole = {
      id: this.editMode ? this.currentUserRoleId! : 0,
      userId: formValue.userId,
      roleId: formValue.roleId,
      statusId: formValue.statusId,
    };

    if (this.editMode) {
      this.userRoleService.updateUserRole(dto).subscribe({
        next: () => {
          this.toastr.success('User role updated successfully');
          this.modalService.dismissAll();
          this.loadUserRoles();
        },
        error: (err: any) => {
          this.toastr.error('Failed to update user role');
          console.error(err);
        }
      });
    } else {
      this.userRoleService.assignUserRole(dto).subscribe({
        next: () => {
          this.toastr.success('User role assigned successfully');
          this.modalService.dismissAll();
          this.loadUserRoles();
        },
        error: (err: any) => {
          this.toastr.error('Failed to assign user role');
          console.error(err);
        }
      });
    }
  }

  deleteUserRole(id: number) {
    if (confirm('Are you sure you want to delete this user role?')) {
      this.userRoleService.deleteUserRole(id).subscribe({
        next: () => {
          this.toastr.success('User role deleted successfully');
          this.loadUserRoles();
        },
        error: (err: any) => {
          this.toastr.error('Failed to delete user role');
          console.error(err);
        }
      });
    }
  }

  getUserName(userId: number): string {
    const user = this.users.find(u => u.id === userId);
    return user ? user.name : 'Unknown';
  }

  getRoleName(roleId: number): string {
    const role = this.roles.find(r => r.id === roleId);
    return role ? role.name : 'Unknown';
  }
}