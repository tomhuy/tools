import { Injectable, signal, computed } from '@angular/core';
import { WeeklyEntry, DaySummary, MOODS, MoodConfig, DisplayMode, FilterMode } from '../../../models/weekly-tracker.model';
import { startOfWeek, eachDayOfInterval, addWeeks, subWeeks, isSameDay, format, startOfDay, addHours, isAfter } from 'date-fns';
import { vi } from 'date-fns/locale';

@Injectable({
  providedIn: 'root'
})
export class ContentExplorerService {
  private entries = signal<WeeklyEntry[]>([]);
  
  // Isolated state
  currentDate = signal<Date>(new Date());
  rangeDays = signal<number>(7);
  
  displayMode = signal<DisplayMode>('both');
  filterMode = signal<FilterMode>('all');
  filterMoodId = signal<string | null>(null);
  filterCategory = signal<string | null>(null);

  categories = signal<string[]>(['Tất cả', 'Công việc', 'Học tập', 'Giải trí', 'Sức khỏe', 'Cá nhân']);

  rangeInterval = computed(() => {
    const date = this.currentDate();
    const days = this.rangeDays();
    const start = startOfWeek(date, { weekStartsOn: 1 });
    const end = addHours(start, (days - 1) * 24 + 23);
    return { start, end };
  });

  dayHeaders = computed<DaySummary[]>(() => {
    const { start, end } = this.rangeInterval();
    const days = eachDayOfInterval({ start, end });
    const today = new Date();

    return days.map((day: Date) => ({
      date: day,
      dayLabel: format(day, 'EEEEEE', { locale: vi }).toUpperCase(),
      dayNumber: day.getDate(),
      dots: [],
      isToday: isSameDay(day, today)
    }));
  });

  constructor() {
    this.generateTechMockData();
  }

  getEntries() {
    return this.entries;
  }

  getEntryAt(date: Date): WeeklyEntry | undefined {
    return this.entries().find(e => e.date.getTime() === date.getTime());
  }

  nextWeek() { this.currentDate.update(d => addWeeks(d, 1)); }
  prevWeek() { this.currentDate.update(d => subWeeks(d, 1)); }
  goToToday() { this.currentDate.set(new Date()); }

  private generateTechMockData() {
    const today = startOfDay(new Date());
    const data: WeeklyEntry[] = [];
    
    const techNews = [
      { tag: 'Show', note: 'Goodbye Tim Apple - daily.dev show (S1E1)', reason: 'Giải trí cuối ngày', cat: 'Giải trí' },
      { tag: 'Adventure', note: 'Your adventure starts now - Web3 & Metaverse', reason: 'Tò mò về công nghệ mới', cat: 'Học tập' },
      { tag: 'Tool', note: 'Tired of Managing Projects the Hard Way? Try ClickUp!', reason: 'Tìm công cụ quản lý task', cat: 'Công việc' },
      { tag: 'Open Source', note: 'Warp terminal goes open-source under AGPL, with OpenAI as founding sponsor', reason: 'Tin tức quan trọng', cat: 'Học tập' },
      { tag: 'UI Design', note: 'DESIGN.md - UI design systems format shared with AI', reason: 'Học cách chuẩn hóa UI', cat: 'Học tập' },
      { tag: 'PHP', note: 'Never type hint on arrays - Best practices', reason: 'Cải thiện kỹ năng coding', cat: 'Công việc' },
      { tag: 'AI', note: 'How AI Changed the Economics of Writing Clean Code', reason: 'Tư duy về AI trong lập trình', cat: 'Công việc' },
      { tag: 'GitHub', note: 'Ghostty is leaving GitHub over reliability concerns', reason: 'Cập nhật tình hình cộng đồng', cat: 'Cá nhân' },
      { tag: 'Economics', note: "AI's Economics Don't Make Sense - A deep dive", reason: 'Phân tích vĩ mô', cat: 'Cá nhân' },
      { tag: 'Career', note: 'Software engineering may no longer be a lifetime career', reason: 'Hơi lo lắng về tương lai', cat: 'Cá nhân' },
      { tag: 'Linux', note: "Linux doesn't need the terminal anymore, and that's actually great", reason: 'Tranh luận thú vị', cat: 'Giải trí' },
      { tag: 'Productivity', note: "Stop Waiting to Feel 'Ready' and Start Building Instead", reason: 'Được truyền động lực', cat: 'Cá nhân' }
    ];

    for (let i = 0; i < 14; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() - i);
      for (let h = 8; h < 23; h++) {
        if (Math.random() > 0.4) {
          const hourDate = new Date(date);
          hourDate.setHours(h, 0, 0, 0);
          if (isAfter(hourDate, new Date())) continue;
          const moodIdx = Math.floor(Math.random() * MOODS.length);
          const news = techNews[Math.floor(Math.random() * techNews.length)];
          data.push({
            id: Math.random().toString(36).substr(2, 9),
            date: hourDate,
            moodId: MOODS[moodIdx].id,
            note: news.note,
            reason: news.reason,
            tags: [news.tag, news.cat]
          });
        }
      }
    }
    this.entries.set(data);
  }
}
