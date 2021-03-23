import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
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
  genderFilterForm: FormGroup;

  constructor(private membersService: MembersService, private fb: FormBuilder,) { }

  ngOnInit(): void {
    this.loadMembers();
    this.initializeForm();
  }

  initializeForm() {
    this.genderFilterForm = this.fb.group({ gender: '' });
    this.genderFilterForm.valueChanges.subscribe(() => this.genderFilterChanged());
  }

  genderFilterChanged() {
    if (this.pagination.gender !== this.genderFilterForm.value.gender) {
      this.userParams.gender = this.genderFilterForm.value.gender;
      this.loadMembers();
    }
  }

  loadMembers() {
    this.membersService.getMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagiantion;
      this.genderFilterForm.setValue({ gender: this.pagination.gender });
    })
  }

  pageChanged(event: any) {
    this.userParams.currentPage = event.page;
    this.loadMembers();
  }
}
