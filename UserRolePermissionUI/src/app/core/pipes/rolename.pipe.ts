// role-name.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { Role } from '../../core/models/role.model';

@Pipe({
  name: 'roleName',
  standalone: true,
})
export class RoleNamePipe implements PipeTransform {
  transform(roleId: number, roles: Role[] = []): string {
    return roles.find((role) => role.id === roleId)?.name ?? 'Unknown';
  }
}
