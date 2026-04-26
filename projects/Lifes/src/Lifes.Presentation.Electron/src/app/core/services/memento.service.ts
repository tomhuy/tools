import { inject, Injectable, signal } from '@angular/core';
import { map, Observable } from 'rxjs';
import { CalendarApiService } from '../../features/monthly-calendar/calendar-api.service';
import { Memento } from '../../models/memento.model';

export interface MementoFilter {
  keyword?: string;
  startDate?: string;
  endDate?: string;
  tagIds?: number[];
}

@Injectable() // No providedIn: 'root' - This is intended to be a local provider
export class MementoService {
  private readonly api = inject(CalendarApiService);

  readonly mementos = signal<Memento[]>([]);
  readonly isLoading = signal(false);
  readonly lastError = signal<string | null>(null);

  loadMementos(filter: MementoFilter = {}) {
    this.isLoading.set(true);
    this.lastError.set(null);

    const params: any = {
      parentOnly: true,
      includeChildren: false
    };

    if (filter.keyword) params.keyword = filter.keyword;
    if (filter.startDate) params.startDate = filter.startDate;
    if (filter.endDate) params.endDate = filter.endDate;
    if (filter.tagIds && filter.tagIds.length > 0) {
      params.tagIds = filter.tagIds.join(',');
    }

    this.api.getMementos(params).subscribe({
      next: (mementos) => {
        this.mementos.set(mementos);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.lastError.set(err.message || 'An error occurred while loading mementos');
        this.isLoading.set(false);
      }
    });
  }

  getTopicDetails(topicId: number, month: number, year: number): Observable<Memento | null> {
    const params = {
      year,
      month,
      includeChildren: true,
      parentOnly: false
    };

    // Map children to parent topic after API returns
    return this.api.getMementos(params).pipe(
      map((mementos: Memento[]) => {
        const parent = mementos.find(m => m.id === topicId);
        if (parent) {
          // Ensure children are linked (in case API returns flat list)
          parent.children = mementos.filter(m => m.parentId === topicId);
          return parent;
        }
        return null;
      })
    );
  }
}
