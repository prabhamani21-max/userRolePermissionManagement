import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { catchError, map, tap, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { jwtDecode } from 'jwt-decode';
import { ToastrService } from 'ngx-toastr';
import {
  ClaimTypes,
  CustomJwtPayload,
} from '../../common/constants/claim-types';
import { DecodedToken } from '../models/decoded-token.model';
import { Observable, of, throwError } from 'rxjs';
import { Role } from '../models/role.model';
import { CreateUser } from '../models/user.model';
import { PermissionService } from './permission.service';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private cookieService = inject(CookieService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private tokenExpirationTimer: any;

  public readonly authSessionKey = 'UserRolePermission_AUTH_TOKEN';
  public readonly decodedTokenKey = 'userDetails';
  public readonly userPermissionsKey = 'userPermissions';
  constructor(private http: HttpClient, private permissionService: PermissionService) {}

  login(email: string, password: string) {
    return this.http
      .post<any>(`${environment.apiUrl}/User/login`, { email, password })
      .pipe(
        tap((response) => {
          if (response.token) {
            this.saveToken(response.token);
            this.startTokenExpirationTimer(); // Start timer on login
          }
        }),
        switchMap((response) => {
           if (response.token) {
             const decoded = this.getDecodedToken();
             console.log('Decoded token in login:', decoded);
             if (decoded && decoded.roleId) {
               console.log('RoleId from decoded:', decoded.roleId);
               const roleId = parseInt(decoded.roleId);
               console.log('Parsed roleId:', roleId);
               if (!isNaN(roleId)) {
                 return this.permissionService.getAllRolePermissions(roleId, undefined, 1, 1, 1000).pipe(
                   tap((permissionsResponse) => {
                     console.log('Permissions response:', permissionsResponse);
                     const permissions = permissionsResponse.items.map((p: any) => p.actionId);
                     console.log('Mapped permissions:', permissions);
                     localStorage.setItem(this.userPermissionsKey, JSON.stringify(permissions));
                   }),
                   map(() => response)
                 );
               }
             }
           }
           return of(response);
         }),
        map((response) => response),
      );
  }

  logout(): void {
    this.clearTokenExpirationTimer(); // Clear timer on logout
    // 1. Remove the auth token from cookies
    this.removeToken();

    // 2. Clear all user-related data from storage
    this.clearAuthData();

    // 3. Navigate to login page
    this.router.navigate(['/userRolePermission/auth/sign-in']);

    // 4. Show logout notification
    this.showLogoutNotification();
  }

  private clearAuthData(): void {
    // Clear localStorage data
    localStorage.removeItem(this.decodedTokenKey);
    localStorage.removeItem(this.userPermissionsKey);
  }

  private showLogoutNotification(): void {
    this.toastr.success('You have been logged out successfully');
  }


  saveToken(token: string): void {
    const expirationDate = new Date();
    expirationDate.setHours(expirationDate.getHours() + 1);

    this.cookieService.set(
      this.authSessionKey,
      token,
      expirationDate,
      '/',
      '',
      true,
      'Strict',
    );
  }
  get token(): string | null {
    return this.cookieService.get(this.authSessionKey) || null;
  }

  removeToken(): void {
    this.cookieService.delete(this.authSessionKey, '/');
  }

  isAuthenticated(): boolean {
    return !!this.token;
  }

  getDecodedToken(): DecodedToken | null {
    const token = this.token;
    if (!token) return null;

    try {
      const decoded = jwtDecode<CustomJwtPayload>(token);
      const mapped: DecodedToken = {
        userId: decoded[ClaimTypes.NAME_IDENTIFIER],
        email: decoded[ClaimTypes.EMAIL],
        name: decoded[ClaimTypes.NAME],
        role: decoded[ClaimTypes.ROLE],
        roleId: decoded.RoleId,
        exp: decoded[ClaimTypes.EXPIRATION],};
      localStorage.setItem(this.decodedTokenKey, JSON.stringify(mapped));
      return mapped;
    } catch (e) {
      console.error('Error decoding token:', e);
      return null;
    }
  }
getUserInformation(): DecodedToken | null {
  const data = localStorage.getItem(this.decodedTokenKey);
  return data ? JSON.parse(data) as DecodedToken : null;
}

getUserPermissions(): number[] {
  const data = localStorage.getItem(this.userPermissionsKey);
  return data ? JSON.parse(data) as number[] : [];
}
  getTokenExpiration(): Date | null {
    const decoded = this.getUserInformation();
    if (!decoded || !decoded.exp) return null;
    return new Date(decoded.exp * 1000);
  }

  isTokenExpired(): boolean {
    const expiration = this.getTokenExpiration();
    if (!expiration) return true;
    return expiration < new Date();
  }
  private startTokenExpirationTimer(): void {
    const expiration = this.getTokenExpiration();
    if (!expiration) return;

    const now = new Date();
    const expiresIn = expiration.getTime() - now.getTime();

    if (expiresIn > 0) {
      this.tokenExpirationTimer = setTimeout(() => {
        this.toastr.warning('Your session has expired. Please log in again.');
        this.logout();
      }, expiresIn);
    } else {
      this.logout(); // Token already expired
    }
  }

  private clearTokenExpirationTimer(): void {
    if (this.tokenExpirationTimer) {
      clearTimeout(this.tokenExpirationTimer);
      this.tokenExpirationTimer = null;
    }
  }
  register(userData: CreateUser): Observable<any> {
    return this.http
      .post<any>(`${environment.apiUrl}/User/register`, userData)
      .pipe(
        catchError((error) => {
          return throwError(() => error);
        }),
      );
  }

  fetchRoles(): Observable<Role[]> {
    return this.http
      .get<Role[]>(`${environment.apiUrl}/Role/GetAllRoles`)
      .pipe(
        tap((response) => {
          console.log('Roles API Response:', response); // Add logging
        }),
        catchError((error) => {
          console.error('Roles API Error:', error); // Add error logging
          return throwError(() => error);
        }),
      );
  }
  checkEmailExists(
    email: string,
    currentUserId: number | null = null,
  ): Observable<{ message: string }>{
    const params: any = { email };
    if (currentUserId !== null) {
      params.currentUserId = currentUserId;
    }
    return this.http
      .get<
        { message: string }
      >(`${environment.apiUrl}/User/checkEmail`, { params: { email } })
      .pipe(
        catchError((error) => {
          // Handle 409 Conflict as a valid response
          if (error.status === 409) {
            return of({
              message: error.error?.Data?.message || 'Email already exists',
            });
          }
        
          console.error('Email check error:', error);
          return of({
            message: 'Error checking email',
          });
        }),
      );
  }

  checkContactExists(
    contactNo: string,
    currentUserId: number | null = null,
  ): Observable<{ message: string }>{
    const params: any = { contactNo };
    if (currentUserId !== null) {
      params.currentUserId = currentUserId;
    }
  
    return this.http
      .get<
       { message: string }
      >(`${environment.apiUrl}/User/checkContact`, { params: { contactNo } })
      .pipe(
        catchError((error) => {
          // Handle 409 Conflict as a valid response
          if (error.status === 409) {
            return of({
              message: error.error?.Data?.message || 'Contact already exists',
            });
          }

          console.error('Contact check error:', error);
          return of({
            message: 'Error checking contact',
          });
        }),
      );
  }
}
