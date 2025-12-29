import {
  Component,
  EventEmitter,
  ChangeDetectionStrategy,
  Input,
  Output,
} from '@angular/core';
import { Role } from '../../../core/models/role.model';
import { CommonModule } from '@angular/common';
import { PageTitleComponent } from '../../../shared/components/page-title/page-title.component';
import { Router } from '@angular/router';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-roletable',
  standalone: true,
  imports: [CommonModule, PageTitleComponent, HasPermissionDirective],
  templateUrl: './roletable.component.html',
  styleUrl: './roletable.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoleTableComponent {
  @Input() roles: Role[] = [];
  @Input() loading = false;

  @Output() deleteRole = new EventEmitter<number>();

  constructor(private router: Router) {}

  // 🔍 View role detail
  onView(role: Role): void {
    this.router.navigate([`userRolePermission/rolemanagement/view/${role.id}`]);
  }

  // 📝 Edit role
  onEdit(role: Role): void {
    this.router.navigate([`userRolePermission/rolemanagement/edit/${role.id}`], {
      state: { role },
    });
  }

  // ➕ Create role
  onCreate(): void {
    this.router.navigate([`userRolePermission/rolemanagement/add`]);
  }

  trackById(index: number, item: Role): number {
    return item.id;
  }

  // 🗑️ Delete role
  onDelete(roleId: number): void {
    this.deleteRole.emit(roleId);
  }

  // Helper to display status
  getStatusText(status: number): string {
    return status === 1 ? 'Active' : status === 0 ? 'Inactive' : 'Unknown';
  }
}