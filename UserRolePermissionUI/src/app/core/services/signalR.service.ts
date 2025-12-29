import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject, BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CookieService } from 'ngx-cookie-service';
import { UserModel } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;
  public connectionEstablished = new BehaviorSubject<boolean>(false);

  // User management
  public userAdded = new Subject<UserModel>();
  public userUpdated = new Subject<UserModel>();
  public userDeleted = new Subject<number>();

  // File management
  public fileUploaded = new Subject<any>();
  public fileDeleted = new Subject<any>();

  constructor(
    private cookieService: CookieService,
    private ngZone: NgZone,
  ) {}

  public startConnection(): void {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.addUserListeners();
      this.addFileListeners();
      return;
    }

    const jwtToken = this.cookieService.get('SALEXIHR_AUTH_TOKEN');
    if (!jwtToken) {
      console.error('JWT token not found. Cannot start SignalR connection.');
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRUrl, {
        accessTokenFactory: () => this.cookieService.get('SALEXIHR_AUTH_TOKEN'),
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.addUserListeners();
        this.addFileListeners();
        this.connectionEstablished.next(true);
      })
      .catch((err: string) =>
        console.error('Error while starting SignalR connection: ' + err),
      );
  }

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection
        .stop()
        .then(() => {
          this.connectionEstablished.next(false);
        })
        .catch((err) =>
          console.error('Error while stopping SignalR connection: ' + err),
        );
    }
  }

  private addUserListeners(): void {
    if (this.hubConnection) {
      this.hubConnection.on('UserAdded', (user: UserModel) => {
        this.ngZone.run(() => this.userAdded.next(user));
      });

      this.hubConnection.on('UserUpdated', (user: UserModel) => {
        this.ngZone.run(() => this.userUpdated.next(user));
      });

      this.hubConnection.on('UserDeleted', (userId: number) => {
        this.ngZone.run(() => this.userDeleted.next(userId));
      });
    }
  }

  private addFileListeners(): void {
    if (this.hubConnection) {
      this.hubConnection.on('FileUploaded', (file: any) => {
        this.ngZone.run(() => this.fileUploaded.next(file));
      });

      this.hubConnection.on('FileDeleted', (data: any) => {
        this.ngZone.run(() => this.fileDeleted.next(data));
      });
    }
  }
}