import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoodTrackerService } from '../weekly-tracker.service';
import { MoodEntryEditorComponent } from '../entry-editor/entry-editor.component';
import { MoodMatrixGridComponent } from '../mood-matrix-grid/mood-matrix-grid.component';
import { IntensityBlocksGridComponent } from '../intensity-blocks-grid/intensity-blocks-grid.component';
import { ViewSelectorBarComponent } from '../view-selector-bar/view-selector-bar.component';
import { RangeTrackerSettingsPanelComponent } from '../range-tracker-settings-panel/range-tracker-settings-panel.component';
import { MoodMetadataManagerComponent } from '../mood-metadata-manager/mood-metadata-manager.component';
import { MOODS, MoodEntry, DisplayMode, FilterMode } from '../../../models/weekly-tracker.model';
import { format, addHours, startOfDay } from 'date-fns';
import { vi } from 'date-fns/locale';

@Component({
  selector: 'app-range-tracker-page',
  standalone: true,
  imports: [CommonModule, MoodEntryEditorComponent, MoodMatrixGridComponent, IntensityBlocksGridComponent, ViewSelectorBarComponent, RangeTrackerSettingsPanelComponent, MoodMetadataManagerComponent],
  templateUrl: './range-tracker-page.component.html',
  styleUrl: './range-tracker-page.component.css'
})
export class RangeTrackerPageComponent implements OnInit, OnDestroy {
  trackerService = inject(MoodTrackerService);

  ranges = [7, 10, 14, 21, 30];
  availableMoods = MOODS;

  selectedEntry = signal<MoodEntry | null>(null);
  isEditorOpen = signal(false);
  isFilterDropdownOpen = signal(false);
  isRangeDropdownOpen = signal(false);
  isSettingsOpen = signal(false);
  isMetadataManagerOpen = signal(false);

  ngOnInit() {
    this.trackerService.rangeDays.set(7);
  }

  ngOnDestroy() {
    this.trackerService.rangeDays.set(7);
  }

  get currentFilterMood() {
    return MOODS.find(m => m.id === this.trackerService.filterMoodId());
  }

  get filterModeLabel() {
    switch (this.trackerService.filterMode()) {
      case 'equal': return '=';
      case 'above': return '≥';
      case 'below': return '≤';
      default: return '';
    }
  }

  get filterBadgeLabel() {
    const mode = this.trackerService.filterMode();
    if (mode === 'all') return null;
    const label = mode === 'equal' ? 'Bằng' : mode === 'above' ? 'Trên ngưỡng' : 'Dưới ngưỡng';
    return `${label} ${this.currentFilterMood?.label || ''} (Chưa mãn)`;
  }

  get weekRangeLabel() {
    const { start, end } = this.trackerService.rangeInterval();
    return `${format(start, 'd/M')} - ${format(end, 'd/M/yyyy')}`;
  }

  setFilterMode(mode: FilterMode) { this.trackerService.filterMode.set(mode); }
  setFilterMood(moodId: string) { this.trackerService.filterMoodId.set(moodId); }
  setRange(days: number) { this.trackerService.rangeDays.set(days); }
  clearFilter() { this.trackerService.filterMode.set('all'); this.trackerService.filterMoodId.set(null); }
  toggleFilterDropdown() { this.isFilterDropdownOpen.update(v => !v); }
  toggleRangeDropdown() { this.isRangeDropdownOpen.update(v => !v); }
  toggleSettings() { this.isSettingsOpen.update(v => !v); }
  setDisplayMode(mode: DisplayMode) { this.trackerService.displayMode.set(mode); }

  onGridCellClick({ day, hour }: { day: Date; hour: number }) {
    const date = addHours(startOfDay(day), hour);
    const entry = this.trackerService.getEntryAt(date) ?? { id: '', date, moodId: 'B', tags: [] };
    this.selectedEntry.set(entry);
    this.isEditorOpen.set(true);
  }

  closeEditor() {
    this.isEditorOpen.set(false);
    this.selectedEntry.set(null);
  }

  openMetadataManager() {
    this.isMetadataManagerOpen.set(true);
  }

  closeMetadataManager() {
    this.isMetadataManagerOpen.set(false);
  }

  onMetadataChanged() {
    // We can reload entries or triggers if needed, but the service handles state.
    // However, the mood entry editor fetches the latest config whenever it's opened.
  }
}
