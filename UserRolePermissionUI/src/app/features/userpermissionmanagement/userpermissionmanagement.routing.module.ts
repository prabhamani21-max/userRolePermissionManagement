import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserPermissionManagementComponent } from './userpermissionmanagement.component';

const routes: Routes = [
  {
    path: '',
    component: UserPermissionManagementComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserPermissionManagementRoutingModule {}