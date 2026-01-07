import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { UserManagementComponent } from './usermanagement.component';
import { UserFormComponent } from '../userform/userform.component';
import { UnsavedChangesGuard } from '../../../core/guards/unsaved-changes.guard';
import { authGuard } from '@/app/core/guards/auth.guard';
import { PermissionGuard } from '@/app/core/guards/permission.guard';

const routes: Routes = [
  {
    path: '',
    component: UserManagementComponent,
  },
  {
    path: 'view/:id',
    component: UserFormComponent,
 
  },
  {
    path: 'add',
   canActivate: [authGuard, PermissionGuard],
   data: { permissions: ['UserManagement:Add'] },

    component: UserFormComponent,
    canDeactivate: [UnsavedChangesGuard],

  },
  {
    path: 'edit/:id',
     canActivate: [authGuard, PermissionGuard],
   data: { permissions: ['UserManagement:Update'] },
    component: UserFormComponent,
    canDeactivate: [UnsavedChangesGuard],
  },
  

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserManagementRoutingModule {}