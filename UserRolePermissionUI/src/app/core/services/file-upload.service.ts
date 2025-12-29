import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  constructor(private http: HttpClient) {}

  downloadFile(entityType: string, entityId: number, fileTypeName: string): Observable<any> {
    const params = { entityType, entityId: entityId.toString(), fileTypeName };
    return this.http.get<any>(`${environment.apiUrl}/File/download`, { params });
  }

  deleteFile(entityType: string, entityId: number, fileTypeName: string): Observable<any> {
    const params = { entityType, entityId: entityId.toString(), fileTypeName };
    return this.http.delete<any>(`${environment.apiUrl}/File/delete`, { params });
  }

  uploadFile(entityType: string, entityId: number, fileTypeName: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('entityType', entityType);
    formData.append('entityId', entityId.toString());
    formData.append('fileTypeName', fileTypeName);

    return this.http.post<any>(`${environment.apiUrl}/File/upload`, formData);
  }
}