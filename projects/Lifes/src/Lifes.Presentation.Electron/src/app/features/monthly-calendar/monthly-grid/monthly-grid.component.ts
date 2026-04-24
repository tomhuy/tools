import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Memento } from '../../../models/memento.model';
import { DisplayMode } from '../../../models/display-mode.model';
import { SelectableMonth } from '../../../models/selectable-month.model';
import { DAYS_IN_MONTH } from '../monthly-calendar.constants';

@Component({
  selector: 'app-monthly-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './monthly-grid.component.html',
  styleUrl: './monthly-grid.component.css'
})
export class MonthlyGridComponent {
  readonly topics = input.required<Memento[]>();
  readonly childrenByParent = input.required<Map<number, Memento[]>>();
  readonly selectedMonths = input.required<SelectableMonth[]>();
  readonly displayMode = input.required<DisplayMode>();
  readonly today = input.required<Date>();

  readonly topicClick = output<Memento>();
  readonly phaseClick = output<Memento>();
  readonly cellClick = output<{ topic: Memento; date: Date }>();

  readonly daysArray = Array.from({ length: DAYS_IN_MONTH }, (_, i) => i + 1);

  isChildInMonth(child: Memento, month: SelectableMonth): boolean {
    const start = new Date(child.startDate);
    const end = new Date(child.endDate);
    
    // Convert to comparable numbers YYYYMM
    const startVal = start.getFullYear() * 100 + (start.getMonth() + 1);
    const endVal = end.getFullYear() * 100 + (end.getMonth() + 1);
    const targetVal = month.year * 100 + month.month;

    return targetVal >= startVal && targetVal <= endVal;
  }

  getStartCol(child: Memento, month: SelectableMonth): number {
    if (this.displayMode() !== 'gantt') {
       const start = new Date(child.startDate);
       return start.getDate();
    }
    const start = new Date(child.startDate);
    const startVal = start.getFullYear() * 100 + (start.getMonth() + 1);
    const targetVal = month.year * 100 + month.month;

    if (startVal < targetVal) return 1;
    return start.getDate();
  }

  getEndCol(child: Memento, month: SelectableMonth): number {
    if (this.displayMode() !== 'gantt') {
        const start = new Date(child.startDate);
        return start.getDate() + 1;
    }
    const end = new Date(child.endDate);
    const endVal = end.getFullYear() * 100 + (end.getMonth() + 1);
    const targetVal = month.year * 100 + month.month;

    if (endVal > targetVal) return DAYS_IN_MONTH + 1;
    return end.getDate() + 1;
  }

  isToday(day: number, month: number, year: number): boolean {
    return this.today().getDate() === day && 
           (this.today().getMonth() + 1) === month &&
           this.today().getFullYear() === year;
  }

  getMonthName(month: number, year: number): string {
    return new Date(year, month - 1).toLocaleString('default', { month: 'long' });
  }

  getWeekday(day: number, month: number, year: number): string {
    const date = new Date(year, month - 1, day);
    const weekdays = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
    return weekdays[date.getDay()];
  }

  shouldShowTitle(child: Memento): boolean {
    const start = new Date(child.startDate);
    const end = new Date(child.endDate);
    const diff = (end.getTime() - start.getTime()) / (1000 * 3600 * 24);
    return diff >= 2; // Only show title if bar is at least 3 days long
  }
}
