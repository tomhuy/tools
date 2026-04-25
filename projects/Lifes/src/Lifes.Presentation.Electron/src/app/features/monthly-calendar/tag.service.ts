import { inject, Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';
import { Tag } from '../../models/tag.model';
import { CalendarApiService } from './calendar-api.service';

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private readonly api = inject(CalendarApiService);

  readonly tags = signal<Tag[]>([]);
  readonly isLoading = signal(false);
  readonly lastError = signal<string | null>(null);

  /**
   * Emits the ID of the deleted tag to notify other services for cascade updates.
   */
  readonly tagDeleted$ = new Subject<number>();

  setTags(tags: Tag[]) {
    this.tags.set(tags);
  }

  loadTags() {
    this.isLoading.set(true);
    this.api.getTags().subscribe({
      next: tags => {
        this.tags.set(tags);
        this.isLoading.set(false);
      },
      error: err => {
        this.lastError.set(err.message ?? 'Failed to load tags');
        this.isLoading.set(false);
      }
    });
  }

  addTag(tag: Tag) {
    this.isLoading.set(true);
    this.api.saveTag(tag).subscribe({
      next: saved => {
        this.tags.update(l => [...l, saved]);
        this.isLoading.set(false);
      },
      error: err => {
        this.lastError.set(err.message ?? 'Failed to add tag');
        this.isLoading.set(false);
      }
    });
  }

  updateTag(tag: Tag) {
    this.isLoading.set(true);
    this.api.saveTag(tag).subscribe({
      next: saved => {
        this.tags.update(l => l.map(x => x.id === saved.id ? saved : x));
        this.isLoading.set(false);
      },
      error: err => {
        this.lastError.set(err.message ?? 'Failed to update tag');
        this.isLoading.set(false);
      }
    });
  }

  deleteTag(id: number) {
    this.isLoading.set(true);
    this.api.deleteTag(id).subscribe({
      next: () => {
        this.tags.update(l => l.filter(x => x.id !== id));
        this.tagDeleted$.next(id);
        this.isLoading.set(false);
      },
      error: err => {
        this.lastError.set(err.message ?? 'Failed to delete tag');
        this.isLoading.set(false);
      }
    });
  }
}
