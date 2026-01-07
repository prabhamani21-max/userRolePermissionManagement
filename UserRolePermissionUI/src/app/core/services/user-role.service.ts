import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseHttpService } from './baseHttp.service';
import { UserRole } from '../models/user-role.model';
import { Pagination } from '../models/pagination.model';

@Injectable({
  providedIn: 'root',
})
export class UserRoleService {
  private baseUrl = environment.apiUrl;

  constructor(private http: BaseHttpService) {}

  assignUserRole(dto: UserRole): Observable<{ message: string; userRoleId: number }> {
    return this.http.post<{ message: string; userRoleId: number }>(`${this.baseUrl}/UserRole/AssignUserRole`, dto);
  }

  getAllUserRoles(
    statusId?: number,
    userId?: number,
    roleId?: number,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<Pagination<UserRole>> {
    let params: any = { pageNumber, pageSize };
    if (statusId !== undefined) {
      params.statusId = statusId;
    }
    if (userId !== undefined) {
      params.userId = userId;
    }
    if (roleId !== undefined) {
      params.roleId = roleId;
    }
    return this.http.get<Pagination<UserRole>>(`${this.baseUrl}/UserRole/GetAllUserRoles`, { params });
  }

  getUserRoleById(id: number): Observable<UserRole> {
    return this.http.get<UserRole>(`${this.baseUrl}/UserRole/GetUserRoleById/${id}`);
  }

  updateUserRole(dto: UserRole): Observable<UserRole> {
    return this.http.put<UserRole>(`${this.baseUrl}/UserRole/UpdateUserRole/${dto.id}`, dto);
  }

  deleteUserRole(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/UserRole/DeleteUserRole/${id}`);
  }

  getUserRolesByUserId(userId: number): Observable<UserRole[]> {
    return this.http.get<UserRole[]>(`${this.baseUrl}/UserRole/GetUserRolesByUserId/${userId}`);
  }
}