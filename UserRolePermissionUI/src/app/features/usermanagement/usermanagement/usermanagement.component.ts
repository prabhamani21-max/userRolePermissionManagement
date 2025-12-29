import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserTableComponent } from '../usertable/usertable.component';
import { UserModel } from '../../../core/models/user.model';
import { Pagination } from '../../../core/models/pagination.model';
import { BehaviorSubject, Observable, take, takeUntil } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from '../../../shared/components/confirm-dialog/confirm-dialog.service';
import { UserService } from '../../../core/services/user.service';
import { RoleService } from '@/app/core/services/role.service';
import { Role } from '../../../core/models/role.model';
import { SignalRService } from '../../../core/services/signalR.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-usermanagement',
  standalone: true,
  imports: [CommonModule, UserTableComponent],
  templateUrl: './usermanagement.component.html',
  styleUrl: './usermanagement.component.scss',
})
export class UserManagementComponent {
  users$ = new BehaviorSubject<UserModel[]>([]); // 👈 reactive user list
  usersObs$ = this.users$.asObservable();
  loading = false;
  message: string = '';
  roles$!: Observable<any>;
  private roles: Role[] = [];
  isSuccess: boolean = false;
  private destroy$ = new Subject<void>();

  // Pagination state
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  constructor(
      private router: Router,
      private toastr: ToastrService,
      private confirmationService: ConfirmationService,
      private userService: UserService,
      private roleService: RoleService,
      private signalRService: SignalRService,
    ) {}
  ngOnInit(): void {
     this.loading = true;
     this.loadUsers();

     this.roles$ = this.roleService.getAllRoles();
     this.roles$.pipe(take(1)).subscribe((res) => {
       this.roles = res || [];
     });
     if (!this.signalRService.connectionEstablished.value) {
       this.signalRService.startConnection();
     }

     this.signalRService.userAdded
       .pipe(takeUntil(this.destroy$))
       .subscribe((user) => {
         // Reload users to maintain pagination
         this.loadUsers();
       });

     this.signalRService.userUpdated
       .pipe(takeUntil(this.destroy$))
       .subscribe((updated) => {
         const current = this.users$.value.map((u) =>
           u.id === updated.id ? updated : u,
         );
         this.users$.next(current);
       });

     this.signalRService.userDeleted
       .pipe(takeUntil(this.destroy$))
       .subscribe((id) => {
         // Remove the user from the list
         const current = this.users$.value.filter((u) => u.id !== id);
         this.users$.next(current);
       });
   }

   loadUsers(): void {
     this.loading = true;

     const paginationParams = {
       roleId: undefined as number | undefined,
       statusId: 1,
       name: undefined as string | undefined,
       pageNumber: this.currentPage,
       pageSize: this.pageSize
     };

     this.userService
       .getAllUsers(
         paginationParams.roleId,
         paginationParams.statusId,
         paginationParams.name,
         paginationParams.pageNumber,
         paginationParams.pageSize
       )
       .pipe(take(1))
       .subscribe({
         next: (res) => this.updateUsersFromResponse(res),
         error: (err) => this.handleLoadUsersError(err)
       });
   }

   private handleLoadUsersError(err: any): void {
     console.error('Error loading users:', err);
     this.toastr.error('Failed to load users. Please try again.');
     this.users$.next([]);
     this.totalCount = 0;
     this.totalPages = 0;
     this.loading = false;
   }

   private updateUsersFromResponse(res: Pagination<UserModel>): void {
     this.users$.next(res.items ?? []);
     this.totalCount = res.totalCount ?? 0;
     this.totalPages = res.totalPages ?? 0;
     this.loading = false;
   }

   onPageChange(newPage: number): void {
     this.currentPage = newPage;
     this.loadUsers();
   }

   onPageSizeChange(newPageSize: number): void {
     this.pageSize = newPageSize;
     this.currentPage = 1; // Reset to first page when page size changes
     this.loadUsers();
   }

   onView(user: UserModel): void {
    this.router.navigate([`userRolePermission/dashboard/user/view/${user.id}`]);
  }
  onEdit(user: UserModel): void {
    this.router.navigate([`userRolePermission/dashboard/user/edit/${user.id}`], {
      state: { user },
    });
  }
ngOnDestroy(): void {
  this.destroy$.next();
  this.destroy$.complete();
}
  // ➕ Create user
  onCreate(): void {
    this.router.navigate([`userRolePermission/dashboard/user/add`]);
  }
  onDelete(id: number): void {
    this.confirmationService
      .confirm('Are you sure?', 'Do you want to delete this user?')
      .then((confirmed) => {
        if (confirmed) {
          this.userService
            .deleteUser(id)
            .pipe(take(1))
            .subscribe({
              next: (response) => {
                this.message =
                  response.message || 'User deleted successfully';
                this.isSuccess = true;
                this.toastr.success(this.message);
                // Remove the user from the list
                const current = this.users$.value.filter((u) => u.id !== id);
                this.users$.next(current);
              },
              error: (err) => {
                // Extract message from the error response
                this.message =
                  err.error?.message ||
                  err.message ||
                  'Failed to delete user';
                this.isSuccess = false;
                this.toastr.error(this.message);
                console.error('Delete error:', err);
              },
            });
        }
      });
  }
}

