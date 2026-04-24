import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProjectFile } from './models/project.model';

export const API_BASE_URL = 'http://localhost:5110/api';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly http = inject(HttpClient);

  scanProjects(path: string): Observable<ProjectFile[]> {
    return this.http.get<ProjectFile[]>(`${API_BASE_URL}/VersionIncrease/scan`, {
      params: { path }
    });
  }
}
