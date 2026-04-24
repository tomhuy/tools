import { Injectable, computed, signal } from '@angular/core';
import { Memento } from '../../models/memento.model';
import { Tag } from '../../models/tag.model';
import { DisplayMode } from '../../models/display-mode.model';
import { SelectableMonth } from '../../models/selectable-month.model';

@Injectable({
  providedIn: 'root'
})
export class MonthlyCalendarService {
  readonly mementos = signal<Memento[]>([]);
  readonly tags = signal<Tag[]>([]);
  readonly displayMode = signal<DisplayMode>('gantt');
  readonly selectedMonths = signal<SelectableMonth[]>([
    { year: 2026, month: 3, label: 'Tháng 3' },
    { year: 2026, month: 4, label: 'Tháng 4' }
  ]);

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

  constructor() {
    this.seedFakeData();
  }

  addChild(child: Memento) {
    this.mementos.update(l => [...l, child]);
  }

  updateMemento(m: Memento) {
    this.mementos.update(l => l.map(x => x.id === m.id ? m : x));
  }

  deleteMemento(id: number) {
    this.mementos.update(l => l.filter(x => x.id !== id));
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

  private seedFakeData() {
    const year = 2026;

    const fakeTags: Tag[] = [
      { id: 1, name: 'Gia đình', color: '#4A90E2' },
      { id: 2, name: 'Học tập', color: '#50E3C2' },
      { id: 3, name: 'Công việc', color: '#F5A623' }
    ];

    const fakeMementos: Memento[] = [
      // Topic 1
      { id: 1, title: 'Vì gia đình', parentId: null, startDate: '2026-03-01', endDate: '2026-04-30', order: 1, color: '#3498db' },
      { id: 11, title: 'X', parentId: 1, startDate: '2026-03-01', endDate: '2026-03-31', order: 1, color: '#3498db' },
      { id: 12, title: 'X', parentId: 1, startDate: '2026-04-01', endDate: '2026-04-15', order: 1, color: '#3498db' },

      // Topic 2
      { id: 2, title: 'Học tâm lý học', parentId: null, startDate: '2026-03-01', endDate: '2026-04-30', order: 2, color: '#27ae60' },
      { id: 21, title: 'Học', parentId: 2, startDate: '2026-03-05', endDate: '2026-03-05', order: 1, color: '#27ae60' },
      { id: 22, title: 'Học', parentId: 2, startDate: '2026-03-12', endDate: '2026-03-12', order: 1, color: '#27ae60' },
      { id: 23, title: 'Học', parentId: 2, startDate: '2026-04-02', endDate: '2026-04-02', order: 1, color: '#27ae60' },

      // Topic 3
      { id: 3, title: 'Đi xem phim', parentId: null, startDate: '2026-03-01', endDate: '2026-04-30', order: 3, color: '#2980b9' },
      { id: 31, title: 'X', parentId: 3, startDate: '2026-03-20', endDate: '2026-03-20', order: 1, color: '#2980b9' },
      { id: 32, title: 'X', parentId: 3, startDate: '2026-04-10', endDate: '2026-04-10', order: 1, color: '#2980b9' },

      // Topic 4
      { id: 4, title: 'Học tư duy', parentId: null, startDate: '2026-03-01', endDate: '2026-04-30', order: 4, color: '#8e44ad' },
      { id: 41, title: 'H...', parentId: 4, startDate: '2026-03-08', endDate: '2026-03-08', order: 1, color: '#8e44ad' },
      { id: 42, title: 'H...', parentId: 4, startDate: '2026-04-05', endDate: '2026-04-06', order: 1, color: '#8e44ad' }
    ];

    this.tags.set(fakeTags);
    this.mementos.set(fakeMementos);
  }
}
