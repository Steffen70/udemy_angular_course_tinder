import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { MemberCacheMap } from '../_models/memberCacheMap';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { LikesParams, PaginationParams, UserParams } from '../_models/paginationParams';
import { AccountService } from './account.service';

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

  private getPaginatiedResult<T>(url: string, paginationParams: PaginationParams) {
    const paginatedResult = new PaginatedResult<T>();
    let params = paginationParams.getPaginationHeaders();

    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body
        var paginationHeaders = response.headers.get('Pagination');
        if (paginationHeaders != null)
          paginatedResult.pagiantion = JSON.parse(paginationHeaders);
        return paginatedResult;
      })
    );
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

    return this.getPaginatiedResult<Member[]>(`${this.baseUrl}users`, this.userParams).pipe(
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
    return this.getPaginatiedResult<Partial<Member[]>>(`${this.baseUrl}likes`, this.likesParams)
  }
}
