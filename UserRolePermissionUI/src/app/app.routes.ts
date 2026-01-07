import { Routes } from '@angular/router'
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component'
import { PrivateLayoutComponent } from './layouts/private-layout/private-layout.component'
import { DashboardComponent } from './features/dashboard/dashboard.component'
import { authGuard } from './core/guards/auth.guard'
import { HRDashboardComponent } from './features/hrdashboard/hrdashboard.component'
import { PermissionGuard } from './core/guards/permission.guard'
import { Unauthorized } from './shared/components/unauthorized/unauthorized.component'

export const routes: Routes = [
    // Redirect empty path to auth/sign-in (login page as default landing)
  {
    path: '',
    redirectTo: 'userRolePermission/auth/sign-in',
    pathMatch: 'full',
  },
   // Auth pages layout
  {
    path: 'userRolePermission/auth',
    component: AuthLayoutComponent,
    loadChildren: () =>
      import('./features/auth/signIn/auth.route').then((m) => m.AUTH_ROUTES),
  },
  // Admin pages layout
  {
    path: 'userRolePermission/admin',
    component: PrivateLayoutComponent,
    canActivate: [authGuard, PermissionGuard],
    data: { permissions: ['AdminDashboard:View'] },
    children: [
      {
        path: '',
        component: DashboardComponent,
      },
    ],
  },
    {
    path: 'userRolePermission/HR',
    component: PrivateLayoutComponent,
    canActivate: [authGuard, PermissionGuard],
    data: { permissions: ['HRDashboard:View'] },
    children: [
      {
        path: '',
        component: HRDashboardComponent,
      },
    ],
  },
  // User Management
  {
    path: 'userRolePermission/users',
    component: PrivateLayoutComponent,
    canActivate: [authGuard,PermissionGuard],
       data: { permissions: ['UserManagement:View'] },
    loadChildren: () => import('./features/usermanagement/usermanagement/usermanagement.module').then(m => m.UserManagementModule)
  },
  // Role Management
  {
    path: 'userRolePermission/rolemanagement',
    component: PrivateLayoutComponent,
    canActivate: [authGuard],
    loadChildren: () => import('./features/rolemanagement/rolemanagement/rolemanagement.module').then(m => m.RoleManagementModule)
  },
  // Role Permission Management
  {
    path: 'userRolePermission/RPManagement',
    component: PrivateLayoutComponent,
    canActivate: [authGuard],
    loadChildren: () => import('./features/rolepermissionmanagement/rolepermissionmanagement/rolepermissionmanagement.routing.module').then(m => m.RolePermissionManagementRoutingModule)
  },
  // User Permission Management
  {
    path: 'userRolePermission/UPManagement',
    component: PrivateLayoutComponent,
    canActivate: [authGuard],
    loadChildren: () => import('./features/userpermissionmanagement/userpermissionmanagement.routing.module').then(m => m.UserPermissionManagementRoutingModule)
  },
  // User Role Management
  {
    path: 'userRolePermission/URManagement',
    component: PrivateLayoutComponent,
    canActivate: [authGuard],
    loadChildren: () => import('./features/userrolemanagement/userrolemanagement.routing.module').then(m => m.UserRoleManagementRoutingModule)
  },
  // Unauthorized access route
  {
    path: 'userRolePermission/unauthorized',
    component: Unauthorized,
  },
    // Fallback route - redirect to sign-in if no route matches
  {
    path: '**',
    redirectTo: 'userRolePermission/auth/sign-in',
  },
]
