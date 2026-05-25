import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { API_BASE_URL } from '../../../api.service';
import { ApiResponse } from '../../../models/api-response.model';
import { MoodMetadataDefinition } from '../../../models/weekly-tracker.model';

@Injectable({
  providedIn: 'root'
})
export class MoodMetadataApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${API_BASE_URL}/moodmetadata`;

  getAll(): Observable<MoodMetadataDefinition[]> {
    return this.http.get<ApiResponse<MoodMetadataDefinition[]>>(this.base)
      .pipe(map(r => this.unwrap(r)));
  }

  save(definition: MoodMetadataDefinition): Observable<MoodMetadataDefinition> {
    return this.http.post<ApiResponse<MoodMetadataDefinition>>(this.base, definition)
      .pipe(map(r => this.unwrap(r)));
  }

  delete(key: string): Observable<void> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/${key}`)
      .pipe(map(r => { this.unwrap(r); }));
  }

  private unwrap<T>(r: ApiResponse<T>): T {
    if (!r.success) {
      throw new Error(r.error ?? 'Unknown API error');
    }
    return r.data as T;
  }
}
