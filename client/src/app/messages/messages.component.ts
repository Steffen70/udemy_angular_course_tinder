import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { MessageParams } from '../_models/paginationParams';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageParams: MessageParams;

  constructor(private messageService: MessageService) {
    this.messageParams = this.messageService.getMessageParams();
  }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.messageService.setMessageParams(this.messageParams);
    this.messageService.getMessages().subscribe(response =>
      [this.messages, this.pagination] = [response.result, response.pagiantion]);
  }

  pageChanged(event: any) {
    this.messageParams.currentPage = event.page;
    this.loadMessages();
  }
}
