import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { RoleManagementComponent } from './rolemanagement.component';
import { RoleFormComponent } from '../roleform/roleform.component';
import { UnsavedChangesGuard } from '../../../core/guards/unsaved-changes.guard';
import { RoleEnum } from '../../../core/enums/role.enum';
const routes: Routes = [
  {
    path: '',
    component: RoleManagementComponent,
  },
  {
    path: 'view/:id',
    component: RoleFormComponent,
  // Admin (2), SuperAdmin (1)
  },
  {
    path: 'add',
    component: RoleFormComponent,
    canDeactivate: [UnsavedChangesGuard],
  // Admin (2), SuperAdmin (1)
  },
  {
    path: 'edit/:id',
    component: RoleFormComponent,
    canDeactivate: [UnsavedChangesGuard],
  // Admin (2), SuperAdmin (1)
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RoleManagementRoutingModule {}
