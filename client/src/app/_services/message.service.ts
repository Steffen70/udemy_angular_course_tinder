import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { MessageParams } from '../_models/paginationParams';
import { getPaginatiedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  messageParams = new MessageParams('Unread');

  constructor(private http: HttpClient) { }

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
}
