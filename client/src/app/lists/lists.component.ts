import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { LikesParams } from '../_models/paginationParams';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  pagination: Pagination;
  likesParams: LikesParams;

  constructor(private membersService: MembersService) {
    this.likesParams = this.membersService.getLikesParams();
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.membersService.setLikesParams(this.likesParams);
    this.membersService.getLikes()
      .subscribe(response => [this.members, this.pagination] = [response.result, response.pagiantion]);
  }

  pageChanged(event: any) {
    this.likesParams.currentPage = event.page;
    this.loadLikes();
  }
}
