import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserRoleManagementComponent } from './userrolemanagement.component';

const routes: Routes = [
  {
    path: '',
    component: UserRoleManagementComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserRoleManagementRoutingModule {}