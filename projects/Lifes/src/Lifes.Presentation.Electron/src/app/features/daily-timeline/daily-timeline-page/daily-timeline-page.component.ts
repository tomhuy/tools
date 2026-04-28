import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DailyTimelineService } from '../daily-timeline.service';
import { ENERGY_LEVELS, EnergyGrade, DailyEntry } from '../../../models/daily-timeline.model';
import { EntryEditorComponent } from '../entry-editor/entry-editor.component';

@Component({
  selector: 'app-daily-timeline-page',
  standalone: true,
  imports: [CommonModule, EntryEditorComponent],
  templateUrl: './daily-timeline-page.component.html',
  styleUrl: './daily-timeline-page.component.css'
})
export class DailyTimelinePageComponent {
  private timelineService = inject(DailyTimelineService);

  currentDate = signal(new Date());
  entries = this.timelineService.entries;
  
  selectedEntry = signal<DailyEntry | null>(null);

  summaryCount = computed(() => {
    return this.entries().filter(e => e.energyGrade).length;
  });

  highestEnergy = computed(() => {
    const filledEntries = this.entries().filter(e => e.energyGrade);
    if (filledEntries.length === 0) return null;
    
    // Grades: A is highest, D is lowest.
    // Index in ENERGY_LEVELS: 0 is A, 7 is D.
    const sorted = [...filledEntries].sort((a, b) => {
      const idxA = ENERGY_LEVELS.findIndex(l => l.grade === a.energyGrade);
      const idxB = ENERGY_LEVELS.findIndex(l => l.grade === b.energyGrade);
      return idxA - idxB;
    });
    
    const best = sorted[0];
    return {
      grade: best.energyGrade,
      time: this.formatHour(best.hour),
      color: ENERGY_LEVELS.find(l => l.grade === best.energyGrade)?.color
    };
  });

  lowestEnergy = computed(() => {
    const filledEntries = this.entries().filter(e => e.energyGrade);
    if (filledEntries.length === 0) return null;
    
    const sorted = [...filledEntries].sort((a, b) => {
      const idxA = ENERGY_LEVELS.findIndex(l => l.grade === a.energyGrade);
      const idxB = ENERGY_LEVELS.findIndex(l => l.grade === b.energyGrade);
      return idxB - idxA;
    });
    
    const worst = sorted[0];
    return {
      grade: worst.energyGrade,
      time: this.formatHour(worst.hour),
      color: ENERGY_LEVELS.find(l => l.grade === worst.energyGrade)?.color
    };
  });

  getLabelForHour(hour: number): string | null {
    if (hour % 3 === 0) {
      return this.formatHour(hour);
    }
    return null;
  }

  formatHour(hour: number): string {
    return `${hour.toString().padStart(2, '0')}:00`;
  }

  getEntryStyle(entry: DailyEntry) {
    if (!entry.energyGrade) return {};
    const level = ENERGY_LEVELS.find(l => l.grade === entry.energyGrade);
    return {
      'background-color': level?.color || '#333',
      'color': '#000'
    };
  }

  getGradeColor(grade: EnergyGrade | undefined) {
    if (!grade) return '#555';
    return ENERGY_LEVELS.find(l => l.grade === grade)?.color;
  }

  navigateDate(days: number) {
    const nextDate = new Date(this.currentDate());
    nextDate.setDate(nextDate.getDate() + days);
    this.currentDate.set(nextDate);
  }

  goToToday() {
    this.currentDate.set(new Date());
  }

  onSlotClick(hour: number) {
    const entry = this.entries().find(e => e.hour === hour);
    if (entry) {
      this.selectedEntry.set({ ...entry });
    }
  }

  onSaveEntry(updatedEntry: DailyEntry) {
    this.timelineService.updateEntry(updatedEntry.hour, updatedEntry);
    this.selectedEntry.set(null);
  }

  onDeleteEntry(hour: number) {
    this.timelineService.deleteEntry(hour);
    this.selectedEntry.set(null);
  }

  onCloseEditor() {
    this.selectedEntry.set(null);
  }
}
