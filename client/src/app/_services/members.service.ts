import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMembers(userParams: UserParams) {
    let params = this.getPaginationHeaders(userParams);
    return this.getPaginatiedResult<Member[]>(`${this.baseUrl}users`, params);
  }

  private getPaginatiedResult<T>(url: string, params: HttpParams) {
    const paginatedResult = new PaginatedResult<T>();

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

  private getPaginationHeaders(userParams: UserParams) {
    let params = new HttpParams();

    for (let [key, value] of Object.entries(userParams)) {
      if (value != null)
        params = params.append(key, value.toString())
    }

    return params
  }

  getMember(username: string) {
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
}
