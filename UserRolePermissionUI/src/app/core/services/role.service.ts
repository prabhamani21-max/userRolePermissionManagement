import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseHttpService } from './baseHttp.service';
import { Role } from '../models/role.model';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private baseUrl = environment.apiUrl;

  constructor(private http: BaseHttpService) {}

  getAllRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(
      `${this.baseUrl}/Role/GetAllRoles`,
    );
  }

  getRoleById(id: number): Observable<Role> {
    return this.http.get<Role>(
      `${this.baseUrl}/Role/GetById/${id}`,
    );
  }

  addEditRole(role: Role): Observable<Role> {
    return this.http.post<Role>(
      `${this.baseUrl}/Role/CreateRole`,
      role,
    );
  }

  deleteRole(id: number): Observable<any> {
    return this.http.delete<any>(
      `${this.baseUrl}/Role/Delete/${id}`,
    );
  }
}