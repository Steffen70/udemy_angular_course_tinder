import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { startHubConnection } from './signalrHelper';
import { take } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = startHubConnection(user, `${this.hubUrl}presence`);

    this.hubConnection.on('UserIsOnline', username => this.onlineUsers$.pipe(take(1))
      .subscribe(usernames => this.onlineUserSource.next([...usernames, username])));

    this.hubConnection.on('UserIsOffline', username => this.onlineUsers$.pipe(take(1))
      .subscribe(usernames => this.onlineUserSource.next([...usernames
        .filter(x => x != username)])));

    this.hubConnection.on('GetOnlineUsers', (arr: string[]) => this.onlineUserSource.next(arr));

    this.hubConnection.on('NewMessageReceived', ({ userName, knownAs }) =>
      this.toastr.info(`${knownAs} has sent you a new message!`).onTap.pipe(take(1))
        .subscribe(() => this.router.navigateByUrl(`/members/${userName}?tab=3`)));
  }

  stopHubConnection() {
    if (this.hubConnection)
      this.hubConnection.stop().catch(error => console.log(error));
  }
}
