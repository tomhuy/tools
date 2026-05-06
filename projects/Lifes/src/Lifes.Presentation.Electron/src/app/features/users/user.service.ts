import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { API_BASE_URL } from '../../api.service';
import { ApiResponse, User } from '../../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly base = `${API_BASE_URL}/users`;

  readonly users = signal<User[]>([]);

  loadUsers(): Observable<User[]> {
    return this.http.get<ApiResponse<User[]>>(this.base).pipe(
      map(r => this.unwrap(r)),
      tap(data => this.users.set(data))
    );
  }

  saveUser(user: User): Observable<User> {
    return this.http.post<ApiResponse<User>>(this.base, user).pipe(
      map(r => this.unwrap(r)),
      tap(() => this.loadUsers().subscribe())
    );
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<ApiResponse<void>>(`${this.base}/${id}`).pipe(
      map(r => this.unwrap(r)),
      tap(() => this.loadUsers().subscribe())
    );
  }

  saveAll(users: User[]): Observable<void> {
    return this.http.post<ApiResponse<void>>(`${this.base}/bulk`, users).pipe(
      map(r => this.unwrap(r)),
      tap(() => this.users.set(users))
    );
  }

  private unwrap<T>(r: ApiResponse<T>): T {
    if (!r.success) {
      throw new Error(r.error ?? 'Unknown API error');
    }
    return r.data as T;
  }
}
