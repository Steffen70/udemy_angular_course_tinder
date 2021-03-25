import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { MemberCacheMap } from '../_models/memberCacheMap';
import { User } from '../_models/user';
import { LikesParams, UserParams } from '../_models/paginationParams';
import { AccountService } from './account.service';
import { getPaginatiedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  memberCache = new MemberCacheMap();
  user: User;
  userParams: UserParams;
  likesParams = new LikesParams('liked');


  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(this.user);
    });
  }


  getLikesParams() {
    return this.likesParams;
  }

  setLikesParams(likesParams: LikesParams) {
    this.likesParams = likesParams;
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers() {
    var cachedData = this.memberCache.get(this.userParams.getIdentifier());
    if (cachedData) return of(cachedData);

    return getPaginatiedResult<Member[]>(`${this.baseUrl}users`, this.http, this.userParams).pipe(
      map(memberArr => {
        this.memberCache.set(this.userParams.getIdentifier(), memberArr);
        return memberArr;
      })
    );
  }

  getMember(username: string) {
    const member = this.memberCache.flattenResults()
      .find((member: Member) => member.userName === username)

    if (member) {
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl + `users/${username}`);
  }

  updateMember(member: Member) {
    return this.http.put<Member>(`${this.baseUrl}users`, member);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(`${this.baseUrl}users/set-main-photo/${photoId}`, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(`${this.baseUrl}users/delete-photo/${photoId}`);
  }

  addLike(username: string) {
    return this.http.post(`${this.baseUrl}likes/${username}`, {});
  }

  getLikes() {
    return getPaginatiedResult<Partial<Member[]>>(`${this.baseUrl}likes`, this.http, this.likesParams)
  }
}
