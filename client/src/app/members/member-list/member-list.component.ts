import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/userParams';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams = new UserParams();

  constructor(private membersService: MembersService) { }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.membersService.getMembers(this.userParams).subscribe(response =>
      [this.members, this.pagination] = [response.result, response.pagiantion]);
  }

  genderFormChanged(event: any) {
    this.userParams.gender = event;
    this.loadMembers();
  }

  pageChanged(event: any) {
    this.userParams.currentPage = event.page;
    this.loadMembers();
  }
}
