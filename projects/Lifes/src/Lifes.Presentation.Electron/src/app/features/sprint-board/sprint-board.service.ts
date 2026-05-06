import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';
import { API_BASE_URL } from '../../api.service';
import { SprintFeature, SprintBoardData, SprintTask } from '../../models/sprint-board.model';
import { ApiResponse } from '../../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class SprintBoardService {
  private readonly http = inject(HttpClient);
  private readonly base = `${API_BASE_URL}/sprintboard`;

  readonly epics = signal<SprintFeature[]>([]);

  loadBoard(): Observable<SprintFeature[]> {
    return this.http.get<ApiResponse<SprintFeature[]>>(this.base).pipe(
      map(r => this.unwrap(r)),
      tap(data => this.epics.set(data))
    );
  }

  saveBoard(epics: SprintFeature[]): Observable<void> {
    return this.http.post<ApiResponse<void>>(this.base, epics).pipe(
      map(r => this.unwrap(r)),
      tap(() => this.epics.set([...epics]))
    );
  }

  private unwrap<T>(r: ApiResponse<T>): T {
    if (!r.success) {
      throw new Error(r.error ?? 'Unknown API error');
    }
    return r.data as T;
  }

  // Helper logic to modify state before saving
  toggleTaskDone(epicId: string, taskId: string): void {
    const current = this.epics();
    const epic = current.find(e => e.id === epicId);
    if (epic) {
      const task = epic.tasks.find(t => t.id === taskId);
      if (task) {
        task.done = !task.done;
        this.saveBoard(current).subscribe();
      }
    }
  }

  moveTask(taskId: string, fromEpicId: string, toEpicId: string, toAssigneeId: string): void {
    const current = this.epics();
    const fromEpic = current.find(e => e.id === fromEpicId);
    if (!fromEpic) return;

    const taskIndex = fromEpic.tasks.findIndex(t => t.id === taskId);
    if (taskIndex === -1) return;

    const [task] = fromEpic.tasks.splice(taskIndex, 1);
    task.assigneeId = toAssigneeId;

    const toEpic = current.find(e => e.id === toEpicId);
    if (toEpic) {
      toEpic.tasks.push(task);
    }

    this.saveBoard(current).subscribe();
  }
}
