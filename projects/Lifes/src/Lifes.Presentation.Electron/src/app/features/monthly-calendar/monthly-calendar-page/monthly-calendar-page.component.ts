import { Component, inject, OnInit, signal } from '@angular/core';
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
export class MonthlyCalendarPageComponent implements OnInit {
  readonly service = inject(MonthlyCalendarService);
  
  readonly showMonthPicker = signal(false);
  readonly showTagPicker = signal(false);

  readonly topics = this.service.topicRows;
  readonly childrenByParent = this.service.childrenByParent;
  readonly selectedMonths = this.service.selectedMonths;
  readonly displayMode = this.service.displayMode;
  readonly today = signal(new Date(2026, 3, 24));

  ngOnInit() {
    this.service.loadInitial(2026);
  }

  readonly months: SelectableMonth[] = Array.from({ length: 12 }, (_, i) => ({
    year: 2026,
    month: i + 1,
    label: `Tháng ${i + 1}`
  }));

  isMonthSelected(monthNum: number, year: number = 2026): boolean {
    return this.selectedMonths().some(m => m.month === monthNum && m.year === year);
  }

  onMonthToggle(monthNum: number, year: number = 2026) {
    this.service.toggleMonth(monthNum, year);
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
    this.service.setDisplayMode(mode);
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
