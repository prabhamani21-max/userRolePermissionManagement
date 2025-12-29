import { Routes } from '@angular/router'
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component'
import { PrivateLayoutComponent } from './layouts/private-layout/private-layout.component'
import { DashboardComponent } from './features/dashboard/dashboard.component'
import { authGuard } from './core/guards/auth.guard'

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
    canActivate: [authGuard],
    children: [
      {
        path: '',
        component: DashboardComponent,
      },
    ],
  },
  // User Management
  {
    path: 'admin/users',
    component: PrivateLayoutComponent,
    canActivate: [authGuard],
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
    // Fallback route - redirect to sign-in if no route matches
  {
    path: '**',
    redirectTo: 'userRolePermission/auth/sign-in',
  },
]
