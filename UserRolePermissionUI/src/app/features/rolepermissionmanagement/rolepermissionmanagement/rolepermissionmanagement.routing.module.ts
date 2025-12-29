import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RolePermissionManagementComponent } from './rolepermissionmanagement.component';

const routes: Routes = [
  {
    path: '',
    component: RolePermissionManagementComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RolePermissionManagementRoutingModule {}