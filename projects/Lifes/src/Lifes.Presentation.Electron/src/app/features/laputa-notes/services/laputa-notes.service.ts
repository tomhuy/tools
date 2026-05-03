import { Injectable, signal, computed, inject, DestroyRef, effect, untracked } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Note, NavItem, NavSection, NoteQuery } from '../models/note.model';
import { LaputaApiService } from './laputa-api.service';
import { Subject, concatMap, tap, catchError, of, switchMap, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LaputaNotesService {
  private api = inject(LaputaApiService);
  private destroyRef = inject(DestroyRef);

  // Colors mapping for sections
  public readonly COLORS: { [key: string]: string } = {
    amber: '#C9B06A', purple: '#7F77DD', teal: '#1D9E75',
    red: '#D4537E', blue: '#378ADD'
  };

  public readonly ALL_TAGS = ['tâm lý học', 'design', 'journal', 'ideas', 'GTD', 'habit', 'đọc sách', 'coding'];

  // State Signals
  public viewMode = signal<'list' | 'card' | 'compact' | 'grid'>('card');
  public theme = signal<'dark' | 'sepia'>('dark');
  public currentSection = signal<string>('all');
  public currentNoteId = signal<string | null>(null);
  public isPreview = signal<boolean>(false);
  public isSidebarOpen = signal<boolean>(true);
  public searchQuery = signal<string>('');
  public sortAsc = signal<boolean>(false);

  // Pagination State
  public currentPage = signal<number>(1);
  public pageSize = signal<number>(100);
  public hasMore = signal<boolean>(true);
  public isLoading = signal<boolean>(false);

  // Data Signals
  public navConfig = signal<{ pinned: NavItem[], sections: NavSection[] }>({
    pinned: [
      { id: 'inbox', label: 'Inbox', icon: 'inbox', badge: 3 },
      { id: 'all', label: 'Tất cả', icon: 'all' },
      { id: 'starred', label: 'Đánh dấu sao', icon: 'star' },
    ],
    sections: [
      {
        id: 'types', label: 'Types', items: [
          { id: 'journal', label: 'Journal', icon: '📓', color: 'amber' },
          { id: 'ideas', label: 'Ý tưởng', icon: '💡', color: 'purple' },
          { id: 'resources', label: 'Tài nguyên', icon: '📚', color: 'teal' },
          { id: 'tasks', label: 'Nhiệm vụ', icon: '✅', color: 'blue' },
          { id: 'psychology', label: 'Tâm lý học', icon: '🧠', color: 'red' },
        ]
      },
      {
        id: 'folders', label: 'Folders', items: [
          { id: 'personal', label: 'Cá nhân', icon: '📁', color: 'amber' },
          { id: 'work', label: 'Công việc', icon: '📁', color: 'blue' },
          { id: 'learning', label: 'Học tập', icon: '📁', color: 'purple' },
        ]
      }
    ]
  });

  public notes = signal<Note[]>([]);

  // Sequential Save Queue
  private saveSubject = new Subject<{ id: string, title: string, content: string }>();

  // Reactive Fetch Trigger
  private fetchSubject = new Subject<{ reset: boolean }>();

  // Sequential Delete Queue
  private deleteSubject = new Subject<string>();

  constructor() {
    // Process save requests sequentially
    this.saveSubject.pipe(
      concatMap(data => {
        console.log(`[Laputa] Saving note ${data.id}...`);
        return this.api.saveNote(data.id, data.title, data.content).pipe(
          tap(updatedNote => {
            console.log(`[Laputa] Note ${data.id} saved successfully.`);
            this.updateNoteInState(updatedNote);
          }),
          catchError(err => {
            console.error(`[Laputa] Failed to save note ${data.id}:`, err);
            return of(null);
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe();

    // Centralized Reactive Fetch Stream
    this.fetchSubject.pipe(
      switchMap(({ reset }) => {
        this.isLoading.set(true);
        if (reset) {
          this.currentPage.set(1);
          this.hasMore.set(true);
        }

        const query: NoteQuery = {
          queryType: untracked(() => this.currentSection() as any),
          page: untracked(() => this.currentPage()),
          pageSize: untracked(() => this.pageSize()),
          search: untracked(() => this.searchQuery()),
          sort: untracked(() => this.sortAsc() ? 'asc' : 'desc')
        };

        return this.api.getNotes(query).pipe(
          map(notes => ({ notes, reset })),
          catchError(err => {
            console.error('[Laputa] Fetch error:', err);
            this.isLoading.set(false);
            return of({ notes: [], reset });
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(({ notes, reset }) => {
      if (notes.length < this.pageSize()) {
        this.hasMore.set(false);
      }

      if (reset) {
        this.notes.set(notes);
      } else {
        this.notes.update(current => [...current, ...notes]);
      }

      this.currentPage.update(p => p + 1);
      this.isLoading.set(false);
    });

    // Sequential Delete Stream
    this.deleteSubject.pipe(
      concatMap(id => {
        console.log(`[Laputa] Deleting note ${id}...`);
        return this.api.deleteNote(id).pipe(
          tap(() => console.log(`[Laputa] Note ${id} deleted from server.`)),
          catchError(err => {
            console.error(`[Laputa] Failed to delete note ${id}:`, err);
            return of(null);
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe();

    // Auto-reload when filters change
    effect(() => {
      // Track these signals
      this.searchQuery();
      this.currentSection();
      this.sortAsc();

      // Reset and fetch - use untracked to avoid tracking signals inside fetchNotes
      untracked(() => this.fetchNotes(true));
    }, { allowSignalWrites: true });
  }

  // Computed properties
  public filteredNotes = computed(() => {
    return this.notes();
  });

  public currentNote = computed(() => {
    const id = this.currentNoteId();
    if (!id) return null;
    return this.notes().find(n => n.id === id) || null;
  });

  // Actions
  public fetchNotes(reset: boolean = false) {
    // If not reset, guard against redundant calls (e.g. scroll)
    if (!reset && (!this.hasMore() || this.isLoading())) return;

    // Trigger the reactive stream
    this.fetchSubject.next({ reset });
  }

  public addNote(title: string, tags: string[]) {
    // In a real app, this would be an API call to create
    // For now, simulate local creation then sync
    const newNote: Note = {
      id: Math.floor(Math.random() * 1000000).toString(),
      title: title || 'Ghi chú mới',
      content: `# ${title || 'Ghi chú mới'}\n\n`,
      tags: [...tags],
      section: (this.currentSection() === 'all' || this.currentSection() === 'inbox') ? null : this.currentSection(),
      starred: false,
      modified: new Date()
    };
    this.notes.update(n => [newNote, ...n]);
    this.currentNoteId.set(newNote.id);

    // Trigger initial save to server
    this.saveNote(newNote.id, newNote.title, newNote.content);
  }

  public saveNote(id: string, title: string, content: string) {
    this.saveSubject.next({ id, title, content });
  }

  private updateNoteInState(updatedNote: Note) {
    this.notes.update(notes => notes.map(n => n.id === updatedNote.id ? updatedNote : n));
  }

  public deleteNote(id: string) {
    // Optimistic Update: Remove from UI immediately
    this.notes.update(notes => notes.filter(n => n.id !== id));
    if (this.currentNoteId() === id) {
      this.currentNoteId.set(null);
    }

    // Trigger the sequential delete stream
    this.deleteSubject.next(id);
  }

  public duplicateNote(id: string) {
    this.api.duplicateNote(id).subscribe(newNote => {
      this.notes.update(n => [newNote, ...n]);
      this.currentNoteId.set(newNote.id);
    });
  }

  public toggleStar(id: string) {
    this.notes.update(notes => notes.map(n => n.id === id ? { ...n, starred: !n.starred } : n));
  }

  public getCountForSection(id: string): number {
    const allNotes = this.notes();
    if (id === 'all') return allNotes.length;
    if (id === 'starred') return allNotes.filter(n => n.starred).length;
    if (id === 'inbox') return allNotes.filter(n => !n.section).length;
    return allNotes.filter(n => n.section === id).length;
  }
}
