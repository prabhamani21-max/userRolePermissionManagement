import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ScreenDto } from '../models/screen.model';
import { ScreenActionDto } from '../models/screen-action.model';

@Injectable({
  providedIn: 'root'
})
export class ScreenService {

  constructor(private http: HttpClient) { }

  getAllScreens(statusId?: number, screenName?: string): Observable<ScreenDto[]> {
    let params = new HttpParams();
    if (statusId !== undefined) {
      params = params.set('statusId', statusId.toString());
    }
    if (screenName) {
      params = params.set('screenName', screenName);
    }
    return this.http.get<ScreenDto[]>(`${environment.apiUrl}/Screen/GetAllScreens`, { params });
  }

  getScreenById(id: number): Observable<ScreenDto> {
    return this.http.get<ScreenDto>(`${environment.apiUrl}/Screen/GetScreenById/${id}`);
  }

  getAllScreenActions(statusId?: number, screenId?: number, actionName?: string): Observable<ScreenActionDto[]> {
    let params = new HttpParams();
    if (statusId !== undefined) {
      params = params.set('statusId', statusId.toString());
    }
    if (screenId !== undefined) {
      params = params.set('screenId', screenId.toString());
    }
    if (actionName) {
      params = params.set('actionName', actionName);
    }
    return this.http.get<ScreenActionDto[]>(`${environment.apiUrl}/ScreenAction/GetAllScreenActions`, { params });
  }

  getScreenActionById(id: number): Observable<ScreenActionDto> {
    return this.http.get<ScreenActionDto>(`${environment.apiUrl}/ScreenAction/GetScreenActionById/${id}`);
  }
}