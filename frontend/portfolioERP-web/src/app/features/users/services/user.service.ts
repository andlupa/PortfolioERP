import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment.development';
import { User } from '../models/user';
import { CreateUserRequest } from '../models/create-user-request';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/users`;

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl);
  }

  updateStatus(
    id: number,
    isActive: boolean
  ): Observable<User> {
    return this.http.patch<User>(
      `${this.apiUrl}/${id}/status`,
      { isActive }
    );
  }

  updateRole(
    id: number,
    role: 'Admin' | 'User'
  ): Observable<User> {
    return this.http.patch<User>(
      `${this.apiUrl}/${id}/role`,
      { role }
    );
  }

  create(
    request: CreateUserRequest
  ): Observable<User> {
    return this.http.post<User>(
      this.apiUrl,
      request
    );
  }
}
