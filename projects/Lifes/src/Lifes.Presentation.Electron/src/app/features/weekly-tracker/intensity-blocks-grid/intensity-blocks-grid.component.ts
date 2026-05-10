import { Component, inject, computed, signal, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoodTrackerService } from '../weekly-tracker.service';
import { MOODS, MoodEntry, MoodConfig, PALETTES } from '../../../models/weekly-tracker.model';
import { addHours, startOfDay, isAfter } from 'date-fns';

@Component({
  selector: 'app-intensity-blocks-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './intensity-blocks-grid.component.html',
  styleUrl: './intensity-blocks-grid.component.css'
})
export class IntensityBlocksGridComponent {
  protected service = inject(MoodTrackerService);
  protected hours = Array.from({ length: 24 }, (_, i) => i);

  cellClick = output<{ day: Date; hour: number }>();

  protected hoveredRow = signal<number | null>(null);
  protected hoveredCol = signal<number | null>(null);

  protected currentPalette = computed(() =>
    PALETTES.find(p => p.id === this.service.palette()) ?? PALETTES[0]
  );

  // For each hour: average mood weight across all days
  protected hourlyAvg = computed(() => {
    const days = this.service.dayHeaders();
    return this.hours.map(hour => {
      const weights = days
        .map(day => this.getEntry(day.date, hour))
        .filter((e): e is MoodEntry => !!e)
        .map(e => MOODS.find(m => m.id === e.moodId)?.weight ?? 0);
      return weights.length ? weights.reduce((a, b) => a + b, 0) / weights.length : 0;
    });
  });

  // Hours with recurring slump: mood weight <= 3 on >= 3 days
  protected slumpHours = computed(() => {
    const days = this.service.dayHeaders();
    return new Set(
      this.hours.filter(hour => {
        const lowCount = days.filter(day => {
          const e = this.getEntry(day.date, hour);
          if (!e) return false;
          return (MOODS.find(m => m.id === e.moodId)?.weight ?? 8) <= 3;
        }).length;
        return lowCount >= 3;
      })
    );
  });

  protected getEntry(day: Date, hour: number): MoodEntry | undefined {
    return this.service.getEntryAt(addHours(startOfDay(day), hour));
  }

  protected isFuture(day: Date, hour: number): boolean {
    return isAfter(addHours(startOfDay(day), hour), new Date());
  }

  protected getCellFg(moodId: string): string {
    const weight = MOODS.find((m: MoodConfig) => m.id === moodId)?.weight ?? 0;
    return this.currentPalette().fg[weight - 1] ?? 'transparent';
  }

  protected getCellBg(moodId: string): string {
    const weight = MOODS.find((m: MoodConfig) => m.id === moodId)?.weight ?? 0;
    return this.currentPalette().bg[weight - 1] ?? 'transparent';
  }

  protected getAvgColor(avgWeight: number): string {
    const idx = Math.round(avgWeight) - 1;
    return this.currentPalette().fg[Math.max(0, idx)] ?? 'transparent';
  }

  protected matchesFilter(moodId: string): boolean {
    const mode = this.service.filterMode();
    const targetId = this.service.filterMoodId();
    if (mode === 'all' || !targetId) return true;
    const w = MOODS.find(m => m.id === moodId)?.weight ?? 0;
    const tw = MOODS.find(m => m.id === targetId)?.weight ?? 0;
    switch (mode) {
      case 'equal': return moodId === targetId;
      case 'above': return w >= tw;
      case 'below': return w <= tw;
      default: return true;
    }
  }

  protected getBlockText(entry: MoodEntry): string {
    switch (this.service.displayMode()) {
      case 'action': return entry.note ?? '';
      case 'reason': return entry.reason ?? '';
      case 'both': return [entry.note, entry.reason].filter(Boolean).join(' · ');
      default: return '';
    }
  }

  protected onCellClick(day: Date, hour: number) {
    if (!this.isFuture(day, hour)) {
      this.cellClick.emit({ day, hour });
    }
  }

  protected onCellEnter(rowIdx: number, colIdx: number) {
    if (this.service.patternAids().alignmentGuidesOnHover) {
      this.hoveredRow.set(rowIdx);
      this.hoveredCol.set(colIdx);
    }
  }

  protected onCellLeave() {
    this.hoveredRow.set(null);
    this.hoveredCol.set(null);
  }
}
