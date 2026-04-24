import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonthlyCalendarService } from '../monthly-calendar.service';
import { MonthlyGridComponent } from '../monthly-grid/monthly-grid.component';
import { SelectableMonth } from '../../../models/selectable-month.model';
import { DisplayMode } from '../../../models/display-mode.model';

@Component({
  selector: 'app-monthly-calendar-page',
  standalone: true,
  imports: [CommonModule, MonthlyGridComponent],
  templateUrl: './monthly-calendar-page.component.html',
  styleUrl: './monthly-calendar-page.component.css'
})
export class MonthlyCalendarPageComponent {
  private readonly calendarService = inject(MonthlyCalendarService);

  readonly topics = this.calendarService.topicRows;
  readonly childrenByParent = this.calendarService.childrenByParent;
  readonly selectedMonths = this.calendarService.selectedMonths;
  readonly displayMode = this.calendarService.displayMode;
  readonly today = signal(new Date(2026, 3, 24));

  readonly showMonthPicker = signal(false);
  readonly showTagPicker = signal(false);

  readonly months: SelectableMonth[] = Array.from({ length: 12 }, (_, i) => ({
    year: 2026,
    month: i + 1,
    label: `Tháng ${i + 1}`
  }));

  isMonthSelected(monthNum: number, year: number = 2026): boolean {
    return this.selectedMonths().some(m => m.month === monthNum && m.year === year);
  }

  onMonthToggle(monthNum: number, year: number = 2026) {
    this.calendarService.toggleMonth(monthNum, year);
  }

  toggleMonthPicker() {
    this.showMonthPicker.update(v => !v);
    if (this.showMonthPicker()) this.showTagPicker.set(false);
  }

  toggleTagPicker() {
    this.showTagPicker.update(v => !v);
    if (this.showTagPicker()) this.showMonthPicker.set(false);
  }

  onDisplayModeChange(mode: DisplayMode) {
    this.calendarService.setDisplayMode(mode);
  }

  onModeSelectChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.onDisplayModeChange(select.value as DisplayMode);
  }

  onTopicClick(topic: any) {
    console.log('Topic clicked:', topic);
  }

  onPhaseClick(phase: any) {
    console.log('Phase clicked:', phase);
  }
}
