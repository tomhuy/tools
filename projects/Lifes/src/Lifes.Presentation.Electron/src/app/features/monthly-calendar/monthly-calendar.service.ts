import { inject, Injectable, computed, signal } from '@angular/core';
import { Memento } from '../../models/memento.model';
import { Tag } from '../../models/tag.model';
import { DisplayMode } from '../../models/display-mode.model';
import { SelectableMonth } from '../../models/selectable-month.model';
import { CalendarApiService, MementoQuery } from './calendar-api.service';
import { TagService } from './tag.service';

@Injectable({
  providedIn: 'root'
})
export class MonthlyCalendarService {
  private readonly api = inject(CalendarApiService);
  private readonly tagService = inject(TagService);

  readonly mementos = signal<Memento[]>([]);
  readonly tags = this.tagService.tags;
  readonly displayMode = signal<DisplayMode>('gantt');
  readonly selectedMonths = signal<SelectableMonth[]>(this.getDefaultQuarterMonths());
  readonly showAchieved = signal<boolean>(false);

  private getDefaultQuarterMonths(): SelectableMonth[] {
    const now = new Date();
    const year = 2026; // Keeping your fixed year for now as per project context
    const month = now.getMonth(); // 0-11
    const quarter = Math.floor(month / 3); // 0, 1, 2, 3
    const startMonth = quarter * 3 + 1; // 1, 4, 7, 10
    
    return [
      { year, month: startMonth, label: `Tháng ${startMonth}` },
      { year, month: startMonth + 1, label: `Tháng ${startMonth + 1}` },
      { year, month: startMonth + 2, label: `Tháng ${startMonth + 2}` }
    ];
  }

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
    this.loadMementos({ year, includeChildren: true });
    this.loadTags();
  }

  public loadTags() {
    this.api.getTags().subscribe(tags => this.tagService.setTags(tags));
  }

  public loadMementos(query: MementoQuery) {
    this.isLoading.set(true);
    this.lastError.set(null);

    this.api.getMementos(query).subscribe({
      next: mementos => {
        this.mementos.set(mementos);
        this.isLoading.set(false);
      },
      error: err => {
        console.error('Failed to load mementos', err);
        this.lastError.set(err.message ?? 'Unknown error');
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Listens for tag deletions to strip the deleted tag ID from all mementos.
   */
  private readonly _cascadeDeleteSubscription = this.tagService.tagDeleted$.subscribe(tagId => {
    this.mementos.update(mementos => 
      mementos.map(m => ({
        ...m,
        tagIds: m.tagIds.filter(id => id !== tagId)
      }))
    );
  });

  addTopic(topic: Memento) {
    this.api.saveMemento(topic).subscribe({
      next: saved => this.addMementoLocal(saved),
      error: err => this.lastError.set(err.message ?? 'Failed to add topic')
    });
  }

  private addMementoLocal(saved: Memento) {
    this.mementos.update(l => {
      if (saved.isAchieved && !this.showAchieved()) return l;
      return [...l, saved];
    });
  }

  addChild(child: Memento) {
    this.api.saveMemento(child).subscribe({
      next: saved => this.addMementoLocal(saved),
      error: err => this.lastError.set(err.message ?? 'Failed to add child')
    });
  }

  updateTopic(topic: Memento) {
    this.api.saveMemento(topic).subscribe({
      next: saved => {
        this.mementos.update(l => {
          // Option B: If the topic is now achieved and we are hiding achieved topics, remove it from the list.
          if (saved.isAchieved && !this.showAchieved()) {
            return l.filter(x => x.id !== saved.id && x.parentId !== saved.id);
          }
          // Otherwise, just update it in the list
          return l.map(x => x.id === saved.id ? saved : x);
        });
      },
      error: err => this.lastError.set(err.message ?? 'Failed to update topic')
    });
  }

  updateMemento(m: Memento) {
    this.api.saveMemento(m).subscribe({
      next: saved => this.mementos.update(l => l.map(x => x.id === saved.id ? saved : x)),
      error: err => this.lastError.set(err.message ?? 'Failed to update memento')
    });
  }

  deleteTopic(id: number) {
    this.api.deleteMemento(id).subscribe({
      next: () => this.mementos.update(l => l.filter(x => x.id !== id && x.parentId !== id)),
      error: err => this.lastError.set(err.message ?? 'Failed to delete topic')
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
