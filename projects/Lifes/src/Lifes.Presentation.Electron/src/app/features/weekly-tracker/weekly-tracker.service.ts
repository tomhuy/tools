import { Injectable, signal, computed } from '@angular/core';
import { WeeklyEntry, DaySummary, MOODS, DisplayMode, MoodConfig, FilterMode } from '../../models/weekly-tracker.model';
import { startOfWeek, endOfWeek, eachDayOfInterval, addWeeks, subWeeks, isSameDay, format, startOfDay, addHours, isAfter } from 'date-fns';
import { vi } from 'date-fns/locale';

@Injectable({
  providedIn: 'root'
})
export class WeeklyTrackerService {
  private entries = signal<WeeklyEntry[]>([]);
  
  // State for navigation
  currentDate = signal<Date>(new Date());
  rangeDays = signal<number>(7); // Default to 7 days
  
  // Filters
  displayMode = signal<DisplayMode>('both');
  filterMode = signal<FilterMode>('all');
  filterMoodId = signal<string | null>(null);
  moodThreshold = signal<string | null>(null);
  filterCategory = signal<string | null>(null);

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
      const dayEntries = this.entries().filter((e: WeeklyEntry) => isSameDay(e.date, day));
      // Take top 5 moods for dots
      const dots = dayEntries
        .slice(0, 5)
        .map((e: WeeklyEntry) => MOODS.find((m: MoodConfig) => m.id === e.moodId)?.color || '#555');

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
    this.generateMockData();
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
  getEntryAt(date: Date): WeeklyEntry | undefined {
    return this.entries().find(e => 
      e.date.getTime() === date.getTime()
    );
  }

  saveEntry(entry: WeeklyEntry) {
    this.entries.update(list => {
      const idx = list.findIndex(e => e.id === entry.id || e.date.getTime() === entry.date.getTime());
      if (idx !== -1) {
        const newList = [...list];
        newList[idx] = entry;
        return newList;
      }
      return [...list, entry];
    });
  }

  deleteEntry(id: string) {
    this.entries.update(list => list.filter(e => e.id !== id));
  }

  private generateMockData() {
    const today = startOfDay(new Date());
    const data: WeeklyEntry[] = [];
    const activities = ['Coding', 'Họp Team', 'Đọc sách', 'Tập thể dục', 'Ăn uống', 'Di chuyển', 'Nghỉ ngơi', 'Xem phim'];
    const reasons = ['Hoàn thành task', 'Căng thẳng', 'Cafe ngon', 'Trời mưa', 'Kẹt xe', 'Ngủ đủ giấc'];
    const categories = ['Công việc', 'Học tập', 'Giải trí', 'Sức khỏe', 'Cá nhân'];

    for (let i = 0; i < 14; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() - i);
      
      for (let h = 8; h < 23; h++) {
        if (Math.random() > 0.4) {
          const hourDate = new Date(date);
          hourDate.setHours(h, 0, 0, 0);
          
          if (isAfter(hourDate, new Date())) continue;

          const moodIdx = Math.floor(Math.random() * MOODS.length);
          const act = activities[Math.floor(Math.random() * activities.length)];
          const res = reasons[Math.floor(Math.random() * reasons.length)];
          const cat = categories[Math.floor(Math.random() * categories.length)];
          
          data.push({
            id: Math.random().toString(36).substr(2, 9),
            date: hourDate,
            moodId: MOODS[moodIdx].id,
            note: 'Hoạt động: ' + act,
            reason: 'Lý do: ' + res,
            tags: [act, cat]
          });
        }
      }
    }

    this.entries.set(data);
  }
}
