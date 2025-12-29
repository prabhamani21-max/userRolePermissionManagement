import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateUser } from '../models/user.model';
import { AuthenticationService } from '../services/auth.service';
import { UserModel } from '../models/user.model';
import { BaseHttpService } from './baseHttp.service';
import { HttpParams } from '@angular/common/http';
import { RoleEnum } from '../enums/role.enum';
import { CookieService } from 'ngx-cookie-service';
import { Pagination } from '../models/pagination.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private authService = inject(AuthenticationService);
  private cookieService = inject(CookieService);

  constructor(private http: BaseHttpService) {}

  getCurrentUser(): Observable<
    (CreateUser & { profileImageUrl: string }) | null
  > {
    const decodedToken = this.authService.getUserInformation();
    if (!decodedToken?.userId) {
      return of(null);
    }

    return this.http
      .get<CreateUser>(`${environment.apiUrl}/User/GetUserById/${decodedToken.userId}`)
      .pipe(
        map((user) => {
          // Construct the derived profileImageUrl (not part of backend model)
          const profileImageUrl = user.profileImage
            ? this.createImageUrl(user.profileImage)
            : 'assets/images/users/dummy-avatar.jpg';

          const userWithImageUrl = {
            ...user,
            profileImageUrl,
          };

          this.currentUserSubject.next(userWithImageUrl);
          return userWithImageUrl;
        }),
        catchError((error) => {
          console.error('Error fetching user:', error);
          return of(null);
        }),
      );
  }

  private createImageUrl(base64String: string): string {
    // Check if already has data URL prefix
    if (base64String.startsWith('data:image')) {
      return base64String;
    }
    if (base64String.startsWith('http') || base64String.startsWith('https'))
      return base64String;
    // Default to JPEG format
    return `data:image/jpeg;base64,${base64String}`;
  }



   getAllUsers(
      roleId?: number,
      statusId?: number,
      name?: string,
      pageNumber: number = 1,
      pageSize: number = 10,
    ): Observable<Pagination<UserModel>> {
     let params = new HttpParams();

     if (roleId !== undefined && roleId !== null) {
       params = params.set('roleId', roleId.toString());
     }

     if (statusId !== undefined && statusId !== null) {
       params = params.set('statusId', statusId.toString());
     }

     if (name !== undefined && name !== null && name.trim() !== '') {
       params = params.set('name', name.trim());
     }

     params = params.set('pageNumber', pageNumber.toString());
     params = params.set('pageSize', pageSize.toString());

     return this.http.get<Pagination<UserModel>>(
       `${this.baseUrl}/User/GetAllUser`,
       { params }, // ✅ pass as part of options object, not HttpParams
     );
   }
  // ✅ Get user by ID
  getUserById(id: number): Observable<UserModel> {
    return this.http.get<UserModel>(
      `${this.baseUrl}/User/GetUserById/${id}`,
    );
  }
  // ✅ Update existing user
  updateUser(
    id: number,
    user: UserModel,
  ): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.baseUrl}/User/UpdateUser/${id}`,
      user,
    );
  }

  // ✅ Delete user
  deleteUser(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.baseUrl}/User/DeleteUser/${id}`,
    );
  }

  getUserIdByEmail(email: string): Observable<number | null> {
    return this.http
      .get<{ data: number }>(
        `${this.baseUrl}/User/GetUserIdByEmail?email=${encodeURIComponent(email)}`,
      )
      .pipe(
        map((response) => {
          if (response?.data) {
            return response.data; // ✅ Correct user ID
          }
          return null;
        }),
        catchError((error) => {
          console.error('Error in getUserIdByEmail:', error);
          return of(null);
        }),
      );


    }
}
