import { Injectable, signal, computed, inject } from '@angular/core';
import { MoodEntry, DaySummary, MOODS, DisplayMode, MoodConfig, FilterMode, ViewMode, PatternAidSettings } from '../../models/weekly-tracker.model';
import { startOfWeek, eachDayOfInterval, addWeeks, subWeeks, isSameDay, format, addHours } from 'date-fns';
import { vi } from 'date-fns/locale';
import { MoodApiService } from './services/mood-api.service';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MoodTrackerService {
  private api = inject(MoodApiService);
  private entries = signal<MoodEntry[]>([]);

  // State for navigation
  currentDate = signal<Date>(new Date());
  rangeDays = signal<number>(7); // Default to 7 days

  // Filters
  displayMode = signal<DisplayMode>('both');
  filterMode = signal<FilterMode>('all');
  filterMoodId = signal<string | null>(null);
  moodThreshold = signal<string | null>(null);
  filterCategory = signal<string | null>(null);

  // View preferences (persist in service — navigation-safe)
  viewMode = signal<ViewMode>('intensity');
  palette = signal<string>('default');
  compactRows = signal<boolean>(false);
  patternAids = signal<PatternAidSettings>({
    hourlyAvgRibbon: true,
    dayMiniSummary: true,
    alignmentGuidesOnHover: true,
    highlightRecurringSlump: true,
  });

  categories = signal<string[]>(['Tất cả', 'Công việc', 'Học tập', 'Giải trí', 'Sức khỏe', 'Cá nhân']);

  // Computed: Range based on rangeDays
  rangeInterval = computed(() => {
    const date = this.currentDate();
    const days = this.rangeDays();
    const start = startOfWeek(date, { weekStartsOn: 1 });
    const end = addHours(start, (days - 1) * 24 + 23); // End of the Nth day
    return { start, end };
  });

  // Computed: Day Headers based on range
  dayHeaders = computed<DaySummary[]>(() => {
    const { start, end } = this.rangeInterval();
    const days = eachDayOfInterval({ start, end });
    const today = new Date();

    return days.map((day: Date) => {
      const dayEntries = this.entries().filter((e: MoodEntry) => isSameDay(new Date(e.date), day));
      // Take top 5 moods for dots
      const dots = dayEntries
        .slice(0, 5)
        .map((e: MoodEntry) => MOODS.find((m: MoodConfig) => m.id === e.moodId)?.color || '#555');

      return {
        date: day,
        dayLabel: format(day, 'EEEEEE', { locale: vi }).toUpperCase(),
        dayNumber: day.getDate(),
        dots,
        isToday: isSameDay(day, today)
      };
    });
  });

  constructor() {
    this.loadEntries();
  }

  async loadEntries() {
    try {
      const data = await firstValueFrom(this.api.getAll());
      // Convert string dates to Date objects if needed, though ISO strings work for comparison with date-fns
      this.entries.set(data);
    } catch (err) {
      console.error('Failed to load mood entries', err);
    }
  }

  // Navigation actions
  nextWeek() {
    this.currentDate.update(d => addWeeks(d, 1));
  }

  prevWeek() {
    this.currentDate.update(d => subWeeks(d, 1));
  }

  goToToday() {
    this.currentDate.set(new Date());
  }

  // Entry actions
  getEntryAt(date: Date): MoodEntry | undefined {
    return this.entries().find(e =>
      new Date(e.date).getTime() === date.getTime()
    );
  }

  async saveEntry(entry: MoodEntry) {
    try {
      const saved = await firstValueFrom(this.api.save(entry));
      this.entries.update(list => {
        const idx = list.findIndex(e => e.id === saved.id || new Date(e.date).getTime() === new Date(saved.date).getTime());
        if (idx !== -1) {
          const newList = [...list];
          newList[idx] = saved;
          return newList;
        }
        return [...list, saved];
      });
    } catch (err) {
      console.error('Failed to save mood entry', err);
    }
  }

  async deleteEntry(id: string) {
    try {
      await firstValueFrom(this.api.delete(id));
      this.entries.update(list => list.filter(e => e.id !== id));
    } catch (err) {
      console.error('Failed to delete mood entry', err);
    }
  }
}
