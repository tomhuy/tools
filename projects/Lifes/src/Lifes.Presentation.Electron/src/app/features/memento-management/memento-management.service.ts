import { inject, Injectable, signal } from '@angular/core';
import { CalendarApiService } from '../monthly-calendar/calendar-api.service';
import { Memento } from '../../models/memento.model';

export interface MementoFilter {
  keyword?: string;
  startDate?: string;
  endDate?: string;
  tagIds?: number[];
  showAchieved?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MementoManagementService {
  private readonly api = inject(CalendarApiService);

  readonly mementos = signal<Memento[]>([]);
  readonly isLoading = signal(false);
  readonly lastError = signal<string | null>(null);
  readonly showAchieved = signal<boolean>(false);

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
      params.tagIds = filter.tagIds;
    }
    
    if (filter.showAchieved !== undefined) {
      params.showAchieved = filter.showAchieved;
    }

    // If no dates provided, fallback to current year to avoid loading everything
    if (!filter.startDate && !filter.endDate) {
      params.year = new Date().getFullYear();
    }

    this.api.getMementos(params).subscribe({
      next: data => {
        this.mementos.set(data);
        this.isLoading.set(false);
      },
      error: err => {
        console.error('Failed to load management mementos', err);
        this.lastError.set(err.message ?? 'Unknown error');
        this.isLoading.set(false);
      }
    });
  }

  saveTopic(topic: Memento) {
    return this.api.saveMemento(topic);
  }

  deleteTopic(id: number) {
    return this.api.deleteMemento(id);
  }

  updateMementoLocal(saved: Memento) {
    this.mementos.update(l => {
      // Option B: If now achieved and we are hiding, remove it.
      if (saved.isAchieved && !this.showAchieved()) {
        return l.filter(x => x.id !== saved.id);
      }
      return l.map(x => x.id === saved.id ? saved : x);
    });
  }

  addMementoLocal(saved: Memento) {
    this.mementos.update(l => {
      // Option B: If now achieved and we are hiding, don't add to list
      if (saved.isAchieved && !this.showAchieved()) {
        return l;
      }
      return [...l, saved];
    });
  }

  removeMementoLocal(id: number) {
    this.mementos.update(l => l.filter(x => x.id !== id));
  }
}
