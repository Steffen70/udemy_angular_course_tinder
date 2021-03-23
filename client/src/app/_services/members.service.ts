import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient, private accountService: AccountService) {
    accountService.currentUser$.subscribe(user => {
      if (user === null) {
        this.members = [];
        this.paginatedResult = new PaginatedResult<Member[]>();
      }
    });
  }

  getMembers(userParams: UserParams) {
    let params = new HttpParams();

    for (let [key, value] of Object.entries(userParams)) {
      if (value != null)
        params = params.append(key, value.toString())
    }

    // if (this.members.length > 0) return of(this.members);
    // return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
    //   map(members => {
    //     this.members = members;
    //     return members
    //   })
    // );

    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map(response => {
        this.paginatedResult.result = response.body
        var paginationHeders = response.headers.get('Pagination');
        if (paginationHeders != null)
          this.paginatedResult.pagiantion = JSON.parse(paginationHeders);
        return this.paginatedResult;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find(m => m.userName === username);
    return member !== undefined ? of(member) : this.http.get<Member>(this.baseUrl + `users/${username}`);
  }

  updateMember(member: Member) {
    return this.http.put<Member>(`${this.baseUrl}users`, member).pipe(map(updatedMember => {
      const index = this.members.indexOf(member);
      this.members[index] = updatedMember;
      return updatedMember;
    }));
  }

  setMainPhoto(photoId: number) {
    return this.http.put(`${this.baseUrl}users/set-main-photo/${photoId}`, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(`${this.baseUrl}users/delete-photo/${photoId}`);
  }
}
