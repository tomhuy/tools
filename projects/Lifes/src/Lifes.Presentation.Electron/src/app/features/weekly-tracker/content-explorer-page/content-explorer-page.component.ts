import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContentExplorerService } from './content-explorer.service';
import { MOODS, WeeklyEntry, MoodConfig, FilterMode } from '../../../models/weekly-tracker.model';
import { format, addHours, startOfDay, isAfter } from 'date-fns';

@Component({
  selector: 'app-content-explorer-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './content-explorer-page.component.html',
  styleUrl: './content-explorer-page.component.css'
})
export class ContentExplorerPageComponent implements OnInit, OnDestroy {
  explorerService = inject(ContentExplorerService);
  
  hours = Array.from({ length: 24 }, (_, i) => i);
  ranges = [7, 10, 14];
  
  // Detail View state
  viewingEntry = signal<WeeklyEntry | null>(null);
  isDetailOpen = signal(false);

  // Filter dropdown state
  isFilterDropdownOpen = signal(false);
  isRangeDropdownOpen = signal(false);
  availableMoods = MOODS;

  ngOnInit() {
    this.explorerService.rangeDays.set(7);
    this.explorerService.displayMode.set('both');
  }

  ngOnDestroy() {
    this.explorerService.rangeDays.set(7);
  }

  get currentFilterMood() {
    const moodId = this.explorerService.filterMoodId();
    return MOODS.find(m => m.id === moodId);
  }

  get filterModeLabel() {
    const mode = this.explorerService.filterMode();
    switch (mode) {
      case 'equal': return '=';
      case 'above': return '≥';
      case 'below': return '≤';
      default: return '';
    }
  }

  get filterBadgeLabel() {
    const mode = this.explorerService.filterMode();
    const mood = this.currentFilterMood;
    if (mode === 'all') return null;
    let label = '';
    if (mode === 'equal') label = 'Bằng';
    if (mode === 'above') label = 'Trên ngưỡng';
    if (mode === 'below') label = 'Dưới ngưỡng';
    return `${label} ${mood?.label || ''}`;
  }

  get weekRangeLabel() {
    const { start, end } = this.explorerService.rangeInterval();
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
    return this.explorerService.getEntryAt(date);
  }

  isFuture(day: Date, hour: number): boolean {
    const date = addHours(startOfDay(day), hour);
    return isAfter(date, new Date());
  }

  matchesFilter(entry: WeeklyEntry | undefined): boolean {
    if (!entry) return false;
    const moodId = entry.moodId;
    const mode = this.explorerService.filterMode();
    const targetMoodId = this.explorerService.filterMoodId();
    const category = this.explorerService.filterCategory();

    // Mood filtering
    let moodMatches = true;
    if (mode !== 'all' && targetMoodId) {
      const currentWeight = MOODS.find(m => m.id === moodId)?.weight || 0;
      const targetWeight = MOODS.find(m => m.id === targetMoodId)?.weight || 0;

      switch (mode) {
        case 'equal': moodMatches = moodId === targetMoodId; break;
        case 'above': moodMatches = currentWeight >= targetWeight; break;
        case 'below': moodMatches = currentWeight <= targetWeight; break;
      }
    }

    // Category filtering
    let categoryMatches = true;
    if (category && category !== 'Tất cả') {
      // For mock data, we match tags. Activity names like 'Coding' -> 'Công việc'
      // We'll do a simple mapping or just check if it's in tags
      categoryMatches = entry.tags.some(t => t.toLowerCase().includes(category.toLowerCase()) || 
                        (category === 'Công việc' && t === 'Coding') ||
                        (category === 'Giải trí' && (t === 'Xem phim' || t === 'Nghỉ ngơi')));
    }

    return moodMatches && categoryMatches;
  }

  setFilterMode(mode: FilterMode) {
    this.explorerService.filterMode.set(mode);
  }

  setFilterMood(moodId: string) {
    this.explorerService.filterMoodId.set(moodId);
  }

  setRange(days: number) {
    this.explorerService.rangeDays.set(days);
  }

  setCategory(category: string) {
    this.explorerService.filterCategory.set(category === 'Tất cả' ? null : category);
  }

  clearFilter() {
    this.explorerService.filterMode.set('all');
    this.explorerService.filterMoodId.set(null);
  }

  toggleFilterDropdown() {
    this.isFilterDropdownOpen.update(v => !v);
  }

  toggleRangeDropdown() {
    this.isRangeDropdownOpen.update(v => !v);
  }

  openDetail(entry: WeeklyEntry) {
    this.viewingEntry.set(entry);
    this.isDetailOpen.set(true);
  }

  closeDetail() {
    this.isDetailOpen.set(false);
    this.viewingEntry.set(null);
  }
}
