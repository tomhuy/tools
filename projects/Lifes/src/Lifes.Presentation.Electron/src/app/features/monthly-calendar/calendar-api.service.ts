import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { API_BASE_URL } from '../../api.service';
import { ApiResponse } from '../../models/api-response.model';
import { Memento } from '../../models/memento.model';
import { Tag } from '../../models/tag.model';

export interface MementoQuery {
  year?: number;
  month?: number;
  tagIds?: number[] | string;
  parentOnly?: boolean;
  includeChildren?: boolean;
  startDate?: string;
  endDate?: string;
  keyword?: string;
  showAchieved?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CalendarApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${API_BASE_URL}/calendar`;

  getMementos(query: MementoQuery): Observable<Memento[]> {
    let params = new HttpParams();
    
    if (query.year !== undefined) params = params.set('year', query.year.toString());
    if (query.month !== undefined) params = params.set('month', query.month.toString());
    
    if (query.tagIds) {
      const tagIdsValue = Array.isArray(query.tagIds) ? query.tagIds.join(',') : query.tagIds;
      params = params.set('tagIds', tagIdsValue);
    }
    
    if (query.parentOnly !== undefined) params = params.set('parentOnly', query.parentOnly.toString());
    if (query.includeChildren !== undefined) params = params.set('includeChildren', query.includeChildren.toString());
    if (query.startDate) params = params.set('startDate', query.startDate);
    if (query.endDate) params = params.set('endDate', query.endDate);
    if (query.keyword) params = params.set('keyword', query.keyword);
    if (query.showAchieved !== undefined) params = params.set('showAchieved', query.showAchieved.toString());

    return this.http.get<ApiResponse<Memento[]>>(`${this.base}/mementos`, { params })
      .pipe(map(r => this.unwrap(r)));
  }

  getTags(): Observable<Tag[]> {
    return this.http.get<ApiResponse<Tag[]>>(`${this.base}/tags`)
      .pipe(map(r => this.unwrap(r)));
  }

  saveMemento(memento: Memento): Observable<Memento> {
    return this.http.post<ApiResponse<Memento>>(`${this.base}/mementos`, memento)
      .pipe(map(r => this.unwrap(r)));
  }

  deleteMemento(id: number): Observable<void> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/mementos/${id}`)
      .pipe(map(r => { this.unwrap(r); }));
  }

  saveTag(tag: Tag): Observable<Tag> {
    return this.http.post<ApiResponse<Tag>>(`${this.base}/tags`, tag)
      .pipe(map(r => this.unwrap(r)));
  }

  deleteTag(id: number): Observable<void> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/tags/${id}`)
      .pipe(map(r => { this.unwrap(r); }));
  }

  private unwrap<T>(r: ApiResponse<T>): T {
    if (!r.success) {
      throw new Error(r.error ?? 'Unknown API error');
    }
    return r.data as T;
  }
}
