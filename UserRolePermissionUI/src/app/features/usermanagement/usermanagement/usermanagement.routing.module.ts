import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { UserManagementComponent } from './usermanagement.component';
import { UserFormComponent } from '../userform/userform.component';
import { UnsavedChangesGuard } from '../../../core/guards/unsaved-changes.guard';
import { RoleGuard } from '../../../core/guards/role.guard';
import { RoleEnum } from '../../../core/enums/role.enum'; // Adjust path as needed

const routes: Routes = [
  {
    path: '',
    component: UserManagementComponent,
  },
  {
    path: 'view/:id',
    component: UserFormComponent,
    canActivate: [RoleGuard],
    data: { roleId: [RoleEnum.Admin, RoleEnum.SuperAdmin] }, // Admin (2), SuperAdmin (1)
  },
  {
    path: 'add',
    component: UserFormComponent,
    canDeactivate: [UnsavedChangesGuard],
    canActivate: [RoleGuard],
    data: { roleId: [RoleEnum.Admin, RoleEnum.SuperAdmin] }, // Admin (2), SuperAdmin (1)
  },
  {
    path: 'edit/:id',
    component: UserFormComponent,
    canDeactivate: [UnsavedChangesGuard],
    canActivate: [RoleGuard],
    data: { roleId: [RoleEnum.Admin, RoleEnum.SuperAdmin] }, // Admin (2), SuperAdmin (1)
  },
  

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserManagementRoutingModule {}