import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { MessageParams } from '../_models/paginationParams';
import { getPaginatiedResult } from './paginationHelper';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { startHubConnection } from './signalrHelper';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  messageParams = new MessageParams('Unread');
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = startHubConnection(user, `${this.hubUrl}message?user=${otherUsername}`);

    this.hubConnection.on('ReceiveMessageThread', m => this.messageThreadSource.next(m));
    this.hubConnection.on('NewMessage', m => this.messageThread$.pipe(take(1)).subscribe(arr => this.messageThreadSource.next([...arr, m])));
  }

  stopHubConnection() {
    if (this.hubConnection)
      this.hubConnection.stop().catch(error => console.log(error));
  }

  getMessageParams() {
    return this.messageParams;
  }

  setMessageParams(messageParams: MessageParams) {
    this.messageParams = messageParams;
  }

  getMessages() {
    return getPaginatiedResult<Message[]>(`${this.baseUrl}messages`, this.http, this.messageParams);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(`${this.baseUrl}messages/thread/${username}`);
  }

  async sendMessage(username: string, content: string) {
    return this.hubConnection.invoke('SendMessage', { recipientUserName: username, content })
      .catch(error => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(`${this.baseUrl}messages/${id}`);
  }
}
