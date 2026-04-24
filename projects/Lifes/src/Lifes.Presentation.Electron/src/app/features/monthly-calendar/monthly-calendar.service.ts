import { inject, Injectable, computed, signal } from '@angular/core';
import { forkJoin } from 'rxjs';
import { Memento } from '../../models/memento.model';
import { Tag } from '../../models/tag.model';
import { DisplayMode } from '../../models/display-mode.model';
import { SelectableMonth } from '../../models/selectable-month.model';
import { CalendarApiService } from './calendar-api.service';

@Injectable({
  providedIn: 'root'
})
export class MonthlyCalendarService {
  private readonly api = inject(CalendarApiService);

  readonly mementos = signal<Memento[]>([]);
  readonly tags = signal<Tag[]>([]);
  readonly displayMode = signal<DisplayMode>('gantt');
  readonly selectedMonths = signal<SelectableMonth[]>([
    { year: 2026, month: 3, label: 'Tháng 3' },
    { year: 2026, month: 4, label: 'Tháng 4' }
  ]);

  readonly isLoading = signal(false);
  readonly lastError = signal<string | null>(null);

  readonly topicRows = computed(() =>
    this.mementos()
      .filter(m => m.parentId === null)
      .sort((a, b) => (a.order - b.order) || a.startDate.localeCompare(b.startDate))
  );

  readonly childrenByParent = computed(() => {
    const map = new Map<number, Memento[]>();
    for (const m of this.mementos()) {
      if (m.parentId !== null) {
        const children = map.get(m.parentId) ?? [];
        map.set(m.parentId, [...children, m]);
      }
    }
    return map;
  });

  loadInitial(year: number) {
    this.isLoading.set(true);
    this.lastError.set(null);

    forkJoin({
      mementos: this.api.getMementos({ year, includeChildren: true }),
      tags: this.api.getTags()
    }).subscribe({
      next: ({ mementos, tags }) => {
        this.mementos.set(mementos);
        this.tags.set(tags);
        this.isLoading.set(false);
      },
      error: err => {
        console.error('Failed to load initial calendar data', err);
        this.lastError.set(err.message ?? 'Unknown error');
        this.isLoading.set(false);
      }
    });
  }

  addChild(child: Memento) {
    this.api.saveMemento(child).subscribe({
      next: saved => this.mementos.update(l => [...l, saved]),
      error: err => this.lastError.set(err.message ?? 'Failed to add child')
    });
  }

  updateMemento(m: Memento) {
    this.api.saveMemento(m).subscribe({
      next: saved => this.mementos.update(l => l.map(x => x.id === saved.id ? saved : x)),
      error: err => this.lastError.set(err.message ?? 'Failed to update memento')
    });
  }

  deleteMemento(id: number) {
    this.api.deleteMemento(id).subscribe({
      next: () => this.mementos.update(l => l.filter(x => x.id !== id)),
      error: err => this.lastError.set(err.message ?? 'Failed to delete memento')
    });
  }

  setDisplayMode(mode: DisplayMode) {
    this.displayMode.set(mode);
  }

  toggleMonth(monthNum: number, year: number = 2026) {
    this.selectedMonths.update(months => {
      const exists = months.find(m => m.month === monthNum && m.year === year);
      if (exists) {
        return months.filter(m => !(m.month === monthNum && m.year === year));
      } else {
        const label = `Tháng ${monthNum}`;
        return [...months, { month: monthNum, year, label }].sort((a, b) => 
          (a.year - b.year) || (a.month - b.month)
        );
      }
    });
  }
}
