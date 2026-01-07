import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseHttpService } from './baseHttp.service';
import { UserPermissionOverride } from '../models/user-permission-override.model';
import { Pagination } from '../models/pagination.model';

@Injectable({
  providedIn: 'root',
})
export class UpoService {
  private baseUrl = environment.apiUrl;

  constructor(private http: BaseHttpService) {}

  createUserPermissionOverride(dto: UserPermissionOverride): Observable<{ message: string; id: number }> {
    return this.http.post<{ message: string; id: number }>(`${this.baseUrl}/UPO/CreateUserPermissionOverride`, dto);
  }

  getAllUserPermissionOverrides(
    statusId?: number,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<Pagination<UserPermissionOverride>> {
    let params: any = { pageNumber, pageSize };
    if (statusId !== undefined) {
      params.statusId = statusId;
    }
    return this.http.get<Pagination<UserPermissionOverride>>(`${this.baseUrl}/UPO/GetAllUserPermissionOverrides`, { params });
  }

  getUserPermissionOverrideById(id: number): Observable<UserPermissionOverride> {
    return this.http.get<UserPermissionOverride>(`${this.baseUrl}/UPO/GetUserPermissionOverrideById/${id}`);
  }

  updateUserPermissionOverride(dto: UserPermissionOverride): Observable<UserPermissionOverride> {
    return this.http.put<UserPermissionOverride>(`${this.baseUrl}/UPO/UpdateUserPermissionOverride/${dto.id}`, dto);
  }

  deleteUserPermissionOverride(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.baseUrl}/UPO/DeleteUserPermissionOverride/${id}`);
  }

  getUserEffectivePermissions(userId: number): Observable<number[]> {
    return this.http.get<number[]>(`${this.baseUrl}/UPO/GetUserEffectivePermissions/${userId}`);
  }
}