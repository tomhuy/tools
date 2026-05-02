import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { API_BASE_URL } from '../../../api.service';
import { ApiResponse } from '../../../models/api-response.model';
import { Note, NoteQuery } from '../models/note.model';

@Injectable({
  providedIn: 'root'
})
export class LaputaApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${API_BASE_URL}/notes`;

  getNotes(query: NoteQuery): Observable<Note[]> {
    let params = new HttpParams()
      .set('queryType', query.queryType)
      .set('page', query.page.toString())
      .set('pageSize', query.pageSize.toString());
    
    if (query.section) params = params.set('section', query.section);
    if (query.search) params = params.set('search', query.search);
    if (query.sort) params = params.set('sort', query.sort);

    return this.http.get<ApiResponse<Note[]>>(this.base, { params })
      .pipe(map(r => this.unwrap(r)));
  }

  saveNote(id: number, title: string, contentHtml: string): Observable<Note> {
    return this.http.post<ApiResponse<Note>>(this.base, { id, title, contentHtml })
      .pipe(map(r => this.unwrap(r)));
  }

  duplicateNote(id: number): Observable<Note> {
    return this.http.post<ApiResponse<Note>>(`${this.base}/${id}/duplicate`, {})
      .pipe(map(r => this.unwrap(r)));
  }

  deleteNote(id: number): Observable<void> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/${id}`)
      .pipe(map(r => { this.unwrap(r); }));
  }

  private unwrap<T>(r: ApiResponse<T>): T {
    if (!r.success) {
      throw new Error(r.error ?? 'Unknown API error');
    }
    return r.data as T;
  }
}
