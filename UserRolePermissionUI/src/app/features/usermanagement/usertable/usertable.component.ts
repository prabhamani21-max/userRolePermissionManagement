import {
  Component,
  EventEmitter,
  ChangeDetectionStrategy,
  Input,
  Output,
  OnInit,
  OnDestroy,
  OnChanges,
  ChangeDetectorRef,
} from '@angular/core';
import { UserModel } from '../../../core/models/user.model';
import { CommonModule } from '@angular/common';
import { PageTitleComponent } from '../../../shared/components/page-title/page-title.component';
import { Router } from '@angular/router';
import { Role } from '../../../core/models/role.model';
import { RoleNamePipe } from '../../../core/pipes/rolename.pipe';
import { AuthenticationService } from '../../../core/services/auth.service';
import { SignalRService } from '../../../core/services/signalR.service';
import { Subject, takeUntil } from 'rxjs';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-usertable',
  standalone: true,
  imports: [CommonModule, PageTitleComponent, RoleNamePipe, NgbPaginationModule, HasPermissionDirective ],
  templateUrl: './usertable.component.html',
  styleUrl: './usertable.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserTableComponent{
  @Input() users: UserModel[] = [];
  @Input() loading = false;
  @Input() roles: Role[] = [];

  // Pagination inputs
  @Input() currentPage = 1;
  @Input() pageSize = 10;
  @Input() totalCount = 0;

  @Output() deleteUser = new EventEmitter<number>();
  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  // Page size options
  pageSizeOptions = [ 10, 25, 50, 100];
  private destroy$ = new Subject<void>();
  constructor(private router: Router,private authService:AuthenticationService,private signalRService: SignalRService, private cdr: ChangeDetectorRef) {}

 

  // 🔍 View user detail (navigate or open modal - you decide)
  onView(user: UserModel): void {
    this.router.navigate([`userRolePermission/dashboard/user/view/${user.id}`]);
  }

  // 📝 Edit user
  onEdit(user: UserModel): void {
    this.router.navigate([`userRolePermission/dashboard/user/edit/${user.id}`], {
      state: { user },
    });
  }

  // ➕ Create user
  onCreate(): void {
    this.router.navigate([`userRolePermission/dashboard/user/add`]);
  }
  trackById(index: number, item: UserModel): number {
    return item.id;
  }
  // 🗑️ Delete user
  onDelete(userId: number): void {
    this.deleteUser.emit(userId);
  }

  // Handle page change
  onPageChange(newPage: number): void {
    this.pageChange.emit(newPage);
  }

  // Handle page size change
  onPageSizeChange(newPageSize: number): void {
    this.pageSizeChange.emit(newPageSize);
  }

  // Calculate end index for pagination info
  getEndIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }


}
