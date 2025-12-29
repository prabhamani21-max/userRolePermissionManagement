import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RolePermissionDto } from '../models/role-permission.model';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {

  constructor(private http: HttpClient) { }

  getAllRolePermissions(roleId?: number, actionId?: number, statusId?: number, pageNumber: number = 1, pageSize: number = 10): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    if (roleId !== undefined) {
      params = params.set('roleId', roleId.toString());
    }
    if (actionId !== undefined) {
      params = params.set('actionId', actionId.toString());
    }
    if (statusId !== undefined) {
      params = params.set('statusId', statusId.toString());
    }
    return this.http.get<any>(`${environment.apiUrl}/RolePermission/GetAllRolePermissions`, { params });
  }

  getRolePermissionById(id: number): Observable<RolePermissionDto> {
    return this.http.get<RolePermissionDto>(`${environment.apiUrl}/RolePermission/GetRolePermissionById/${id}`);
  }

  createRolePermission(dto: RolePermissionDto): Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/RolePermission/CreateRolePermission`, dto);
  }

  updateRolePermission(dto: RolePermissionDto): Observable<RolePermissionDto> {
    return this.http.put<RolePermissionDto>(`${environment.apiUrl}/RolePermission/UpdateRolePermission`, dto);
  }

  deleteRolePermission(id: number): Observable<any> {
    return this.http.delete<any>(`${environment.apiUrl}/RolePermission/DeleteRolePermission/${id}`);
  }
}