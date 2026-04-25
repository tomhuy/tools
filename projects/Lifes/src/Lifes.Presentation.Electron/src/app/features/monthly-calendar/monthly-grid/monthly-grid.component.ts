import { Component, input, output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Memento } from '../../../models/memento.model';
import { DisplayMode } from '../../../models/display-mode.model';
import { SelectableMonth } from '../../../models/selectable-month.model';
import { DAYS_IN_MONTH } from '../monthly-calendar.constants';
import { getSolidBgColor, getSolidFgColor, STANDARD_COLOR_PALETTE } from '../../../utils/color-utils';
import { MonthlyCalendarService } from '../monthly-calendar.service';

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
  readonly includeChildren = input<boolean>(true);
  readonly showTimeline = input<boolean>(false);
  readonly forceShowTooltips = input<boolean>(false);
  readonly isVertical = input<boolean>(false);

  readonly topicClick = output<Memento>();
  readonly phaseClick = output<Memento>();
  readonly cellClick = output<{ topic: Memento; date: Date }>();

  private readonly calendarService = inject(MonthlyCalendarService);

  readonly daysArray = Array.from({ length: DAYS_IN_MONTH }, (_, i) => i + 1);
  readonly presetColors = STANDARD_COLOR_PALETTE;

  readonly activePopup = signal<{
    memento: Memento;
    position: { x: number; y: number };
    isNew: boolean;
  } | null>(null);

  readonly activeColorPicker = signal<{
    memento: Memento;
    position: { x: number; y: number };
  } | null>(null);

  isMementoInMonth(memento: Memento, month: SelectableMonth): boolean {
    const start = new Date(memento.startDate);
    const end = new Date(memento.endDate);

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

  getBgColor(memento: Memento, fallback?: Memento): string {
    return getSolidBgColor(memento.color || fallback?.color);
  }

  getFgColor(memento: Memento, fallback?: Memento): string {
    return getSolidFgColor(memento.color || fallback?.color);
  }

  onGridCellClick(event: MouseEvent, topic: Memento, day: number, month: number, year: number): void {
    const dateStr = `${year}-${month.toString().padStart(2, '0')}-${day.toString().padStart(2, '0')}`;

    // Check if there is already a phase on this exact day for this topic
    const existing = (this.childrenByParent().get(topic.id) ?? [])
      .find(m => m.startDate.split('T')[0] === dateStr);

    if (existing) {
      this.onPhaseClick(event, existing);
      return;
    }

    const newPhase: Memento = {
      id: 0,
      title: 'X',
      startDate: dateStr,
      endDate: dateStr,
      parentId: topic.id,
      order: 0,
      tagIds: [],
      color: topic.color
    };

    this.activePopup.set({
      memento: newPhase,
      position: { x: event.clientX, y: event.clientY },
      isNew: true
    });
    this.activeColorPicker.set(null);
  }

  onPhaseClick(event: MouseEvent, child: Memento): void {
    event.stopPropagation();
    this.activePopup.set({
      memento: { ...child },
      position: { x: event.clientX, y: event.clientY },
      isNew: false
    });
    this.activeColorPicker.set(null);
  }

  onOpenColorPicker(event: MouseEvent, child: Memento): void {
    event.stopPropagation();
    this.activeColorPicker.set({
      memento: child,
      position: { x: event.clientX, y: event.clientY }
    });
    this.activePopup.set(null);
  }

  onSelectColor(memento: Memento, color: string): void {
    const updated = { ...memento, color };
    this.calendarService.updateMemento(updated);
    this.closeColorPicker();
  }

  onUpdatePhase(): void {
    const popup = this.activePopup();
    if (!popup) return;

    if (popup.isNew) {
      this.calendarService.addChild(popup.memento);
    } else {
      this.calendarService.updateMemento(popup.memento);
    }
    this.closePopup();
  }

  onDeletePhase(id: number): void {
    this.calendarService.deleteMemento(id);
    this.closePopup();
  }

  closePopup(): void {
    this.activePopup.set(null);
  }

  closeColorPicker(): void {
    this.activeColorPicker.set(null);
  }
}
