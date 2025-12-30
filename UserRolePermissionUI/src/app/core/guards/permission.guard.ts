import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router,
} from '@angular/router';
import { AuthenticationService } from '../services/auth.service';
import { ScreenService } from '../services/screen.service';
import { map, switchMap, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PermissionGuard implements CanActivate {
  constructor(
    private router: Router,
    private authService: AuthenticationService,
    private screenService: ScreenService,
  ) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
  ) {
    const requiredPermissions: string[] = next.data['permissions'];

    if (!requiredPermissions || requiredPermissions.length === 0) {
      return of(true); // No permissions required, allow access
    }

    const userPermissions = this.authService.getUserPermissions();

    if (!userPermissions || userPermissions.length === 0) {
      console.warn('No permissions found for user');
      return of(this.router.parseUrl('/userRolePermission/unauthorized'));
    }

    return this.screenService.getAllScreenActions().pipe(
      map(actions => {
        const requiredIds = requiredPermissions.map(key => {
          const action = actions.find(a => a.key === key);
          return action ? action.id.toString() : null;
        }).filter(id => id !== null) as string[];

        const hasPermission = requiredIds.some(id => userPermissions.includes(id));

        if (hasPermission) {
          return true;
        }

        console.warn(
          `User does not have required permissions: ${requiredPermissions}. User permissions: ${userPermissions}`,
        );
        return this.router.parseUrl('/userRolePermission/unauthorized');
      })
    );
  }
}