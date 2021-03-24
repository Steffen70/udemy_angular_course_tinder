import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/paginationParams';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  genderList = [{ value: 'male', display: 'Man' }, { value: 'female', display: 'Woman' }]

  constructor(private membersService: MembersService) {
    this.userParams = this.membersService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.membersService.setUserParams(this.userParams);
    this.membersService.getMembers().subscribe(response =>
      [this.members, this.pagination] = [response.result, response.pagiantion]);
  }

  resetFilters() {
    this.userParams = this.membersService.resetUserParams();
  }

  pageChanged(event: any) {
    this.userParams.currentPage = event.page;
    this.loadMembers();
  }
}
