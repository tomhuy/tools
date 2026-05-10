import { Component, inject, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoodTrackerService } from '../weekly-tracker.service';
import { MOODS, MoodEntry, MoodConfig } from '../../../models/weekly-tracker.model';
import { addHours, startOfDay, isAfter } from 'date-fns';

@Component({
  selector: 'app-mood-matrix-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mood-matrix-grid.component.html',
  styleUrl: './mood-matrix-grid.component.css'
})
export class MoodMatrixGridComponent {
  protected trackerService = inject(MoodTrackerService);
  cellClick = output<{ day: Date; hour: number }>();

  protected hours = Array.from({ length: 24 }, (_, i) => i);

  protected getMoodColor(moodId: string): string {
    return MOODS.find((m: MoodConfig) => m.id === moodId)?.color || 'transparent';
  }

  protected getMoodLabel(moodId: string): string {
    return MOODS.find((m: MoodConfig) => m.id === moodId)?.label || '';
  }

  protected getEntry(day: Date, hour: number): MoodEntry | undefined {
    return this.trackerService.getEntryAt(addHours(startOfDay(day), hour));
  }

  protected isFuture(day: Date, hour: number): boolean {
    return isAfter(addHours(startOfDay(day), hour), new Date());
  }

  protected matchesFilter(moodId: string | undefined): boolean {
    if (!moodId) return false;
    const mode = this.trackerService.filterMode();
    const targetId = this.trackerService.filterMoodId();
    if (mode === 'all' || !targetId) return true;

    const w = MOODS.find(m => m.id === moodId)?.weight || 0;
    const tw = MOODS.find(m => m.id === targetId)?.weight || 0;
    switch (mode) {
      case 'equal': return moodId === targetId;
      case 'above': return w >= tw;
      case 'below': return w <= tw;
      default: return true;
    }
  }

  protected onCellClick(day: Date, hour: number) {
    if (this.isFuture(day, hour)) return;
    this.cellClick.emit({ day, hour });
  }
}
