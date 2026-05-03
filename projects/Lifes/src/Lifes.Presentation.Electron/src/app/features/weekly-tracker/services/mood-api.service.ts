import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { API_BASE_URL } from '../../../api.service';
import { ApiResponse } from '../../../models/api-response.model';
import { MoodEntry } from '../../../models/weekly-tracker.model';

@Injectable({
  providedIn: 'root'
})
export class MoodApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${API_BASE_URL}/mood`;

  getAll(): Observable<MoodEntry[]> {
    return this.http.get<ApiResponse<MoodEntry[]>>(this.base)
      .pipe(map(r => this.unwrap(r)));
  }

  getByRange(start: Date, end: Date): Observable<MoodEntry[]> {
    const params = new HttpParams()
      .set('start', start.toISOString())
      .set('end', end.toISOString());
    
    return this.http.get<ApiResponse<MoodEntry[]>>(`${this.base}/range`, { params })
      .pipe(map(r => this.unwrap(r)));
  }

  save(entry: MoodEntry): Observable<MoodEntry> {
    // Convert date to ISO string before sending
    const payload = {
      ...entry,
      date: entry.date.toISOString()
    };
    return this.http.post<ApiResponse<MoodEntry>>(this.base, payload)
      .pipe(map(r => this.unwrap(r)));
  }

  delete(id: string): Observable<void> {
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
