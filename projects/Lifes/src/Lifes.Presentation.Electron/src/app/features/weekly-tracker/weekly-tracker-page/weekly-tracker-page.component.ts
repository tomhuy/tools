import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WeeklyTrackerService } from '../weekly-tracker.service';
import { WeeklyEntryEditorComponent } from '../entry-editor/entry-editor.component';
import { MOODS, WeeklyEntry, DisplayMode, MoodConfig, FilterMode } from '../../../models/weekly-tracker.model';
import { format, addHours, startOfDay, isAfter } from 'date-fns';
import { vi } from 'date-fns/locale';

@Component({
  selector: 'app-weekly-tracker-page',
  standalone: true,
  imports: [CommonModule, WeeklyEntryEditorComponent],
  templateUrl: './weekly-tracker-page.component.html',
  styleUrl: './weekly-tracker-page.component.css'
})
export class WeeklyTrackerPageComponent {
  trackerService = inject(WeeklyTrackerService);
  
  hours = Array.from({ length: 24 }, (_, i) => i);
  
  // Modal state
  selectedEntry = signal<WeeklyEntry | null>(null);
  isEditorOpen = signal(false);

  // Filter dropdown state
  isFilterDropdownOpen = signal(false);
  availableMoods = MOODS;

  get currentFilterMood() {
    const moodId = this.trackerService.filterMoodId();
    return MOODS.find(m => m.id === moodId);
  }

  get filterModeLabel() {
    const mode = this.trackerService.filterMode();
    switch (mode) {
      case 'equal': return '=';
      case 'above': return '≥';
      case 'below': return '≤';
      default: return '';
    }
  }

  get filterBadgeLabel() {
    const mode = this.trackerService.filterMode();
    const mood = this.currentFilterMood;
    if (mode === 'all') return null;
    let label = '';
    if (mode === 'equal') label = 'Bằng';
    if (mode === 'above') label = 'Trên ngưỡng';
    if (mode === 'below') label = 'Dưới ngưỡng';
    return `${label} ${mood?.label || ''} (Chưa mãn)`;
  }

  get weekRangeLabel() {
    const { start, end } = this.trackerService.rangeInterval();
    return `${format(start, 'd/M')} - ${format(end, 'd/M/yyyy')}`;
  }

  getMoodColor(moodId: string) {
    return MOODS.find((m: MoodConfig) => m.id === moodId)?.color || 'transparent';
  }

  getMoodLabel(moodId: string) {
    return MOODS.find((m: MoodConfig) => m.id === moodId)?.label || '';
  }

  getEntry(day: Date, hour: number): WeeklyEntry | undefined {
    const date = addHours(startOfDay(day), hour);
    return this.trackerService.getEntryAt(date);
  }

  isFuture(day: Date, hour: number): boolean {
    const date = addHours(startOfDay(day), hour);
    return isAfter(date, new Date());
  }

  matchesFilter(moodId: string | undefined): boolean {
    if (!moodId) return false;
    const mode = this.trackerService.filterMode();
    const targetMoodId = this.trackerService.filterMoodId();
    
    if (mode === 'all') return true;
    if (!targetMoodId) return true;

    const currentWeight = MOODS.find(m => m.id === moodId)?.weight || 0;
    const targetWeight = MOODS.find(m => m.id === targetMoodId)?.weight || 0;

    switch (mode) {
      case 'equal': return moodId === targetMoodId;
      case 'above': return currentWeight >= targetWeight;
      case 'below': return currentWeight <= targetWeight;
      default: return true;
    }
  }

  setFilterMode(mode: FilterMode) {
    this.trackerService.filterMode.set(mode);
    if (mode !== 'all' && !this.trackerService.filterMoodId()) {
      this.trackerService.filterMoodId.set('B'); // Default to B if not set
    }
  }

  setFilterMood(moodId: string) {
    this.trackerService.filterMoodId.set(moodId);
    if (this.trackerService.filterMode() === 'all') {
      this.trackerService.filterMode.set('equal');
    }
  }

  clearFilter() {
    this.trackerService.filterMode.set('all');
    this.trackerService.filterMoodId.set(null);
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen.update(v => !v);
  }

  onCellClick(day: Date, hour: number) {
    if (this.isFuture(day, hour)) return;
    
    const date = addHours(startOfDay(day), hour);
    let entry = this.trackerService.getEntryAt(date);
    
    if (!entry) {
      entry = {
        id: '',
        date: date,
        moodId: 'B', // Default
        tags: []
      };
    }
    
    this.selectedEntry.set(entry);
    this.isEditorOpen.set(true);
  }

  closeEditor() {
    this.isEditorOpen.set(false);
    this.selectedEntry.set(null);
  }

  setDisplayMode(mode: DisplayMode) {
    this.trackerService.displayMode.set(mode);
  }
}
