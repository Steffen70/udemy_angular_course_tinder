import { AfterViewChecked, ChangeDetectionStrategy, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit, AfterViewChecked {
  @ViewChild('scrollMe') scrollMe: ElementRef;
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() username: string;
  messageContent: string;
  messages: Message[] = [];
  scroll: boolean = true;

  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
    this.messageService.messageThread$.subscribe(m => {
      this.messages = m;
      this.scroll = true;
    });
  }

  ngAfterViewChecked(): void {
    this.scrollBottom();
  }

  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).then(() => this.messageForm.reset());
  }

  scrollBottom() {
    if (this.scroll && this.scrollMe?.nativeElement) {
      this.scroll = false;
      this.scrollMe.nativeElement.scrollTop = this.scrollMe.nativeElement.scrollHeight;
    }
  }
}
