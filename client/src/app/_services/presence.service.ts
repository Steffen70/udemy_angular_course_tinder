import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { startHubConnection } from './signalrHelper';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService) { }

  createHubConnection(user: User) {
    this.hubConnection = startHubConnection(user, `${this.hubUrl}presence`);

    this.hubConnection.on('UserIsOnline', username => this.toastr.info(`${username} has connected`));
    this.hubConnection.on('UserIsOffline', username => this.toastr.warning(`${username} has disconnected`));

    this.hubConnection.on('GetOnlineUsers', (arr: string[]) => this.onlineUserSource.next(arr));
  }

  stopHubConnection() {
    if (this.hubConnection)
      this.hubConnection.stop().catch(error => console.log(error));
  }
}
