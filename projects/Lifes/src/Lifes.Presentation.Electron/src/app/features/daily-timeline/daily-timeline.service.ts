import { Injectable, signal } from '@angular/core';
import { DailyEntry, EnergyGrade } from '../../models/daily-timeline.model';

@Injectable({
  providedIn: 'root'
})
export class DailyTimelineService {
  private _entries = signal<DailyEntry[]>(this.generateMockEntries());

  entries = this._entries.asReadonly();

  constructor() {}

  getEntriesForDate(date: Date): DailyEntry[] {
    // For now, return the same mock data regardless of date
    return this._entries();
  }

  updateEntry(hour: number, entry: Partial<DailyEntry>) {
    this._entries.update(current => {
      const index = current.findIndex(e => e.hour === hour);
      if (index !== -1) {
        const updated = [...current];
        updated[index] = { ...updated[index], ...entry };
        return updated;
      }
      return current;
    });
  }

  deleteEntry(hour: number) {
    this._entries.update(current => {
      const index = current.findIndex(e => e.hour === hour);
      if (index !== -1) {
        const updated = [...current];
        updated[index] = { hour, tags: [] }; // Reset to empty slot
        return updated;
      }
      return current;
    });
  }

  private generateMockEntries(): DailyEntry[] {
    const entries: DailyEntry[] = [];
    for (let i = 0; i < 24; i++) {
      entries.push({ hour: i, tags: [] });
    }

    // Add some mock data as seen in the image
    entries[0] = {
      hour: 0,
      energyGrade: EnergyGrade.B_PLUS,
      tags: ['Làm việc một mình', 'Ở cùng gia đình', 'Đọc sách', 'Di chuyển', 'Sáng tạo'],
      note: 'hello'
    };
    entries[1] = {
      hour: 1,
      energyGrade: EnergyGrade.B,
      tags: [],
      note: 'tuyệt vời'
    };
    entries[2] = {
      hour: 2,
      energyGrade: EnergyGrade.B_PLUS,
      tags: ['Tập thể dục', 'Coding'],
      note: 'tập thể dục'
    };
    entries[3] = {
      hour: 3,
      energyGrade: EnergyGrade.B_MINUS,
      tags: ['Di chuyển', 'Đọc sách', 'Ở cùng gia đình'],
      note: ''
    };

    return entries;
  }
}
