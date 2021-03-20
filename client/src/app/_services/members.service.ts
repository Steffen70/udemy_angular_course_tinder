import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members
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
}
