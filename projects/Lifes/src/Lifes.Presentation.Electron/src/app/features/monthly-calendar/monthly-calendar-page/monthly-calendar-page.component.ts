import { Component, inject, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MonthlyCalendarService } from '../monthly-calendar.service';
import { MonthlyGridComponent } from '../monthly-grid/monthly-grid.component';
import { TopicEditorComponent } from '../topic-editor/topic-editor.component';
import { TagManagementComponent } from '../tag-management/tag-management.component';
import { SelectableMonth } from '../../../models/selectable-month.model';
import { DisplayMode } from '../../../models/display-mode.model';
import { Memento } from '../../../models/memento.model';

@Component({
  selector: 'app-monthly-calendar-page',
  standalone: true,
  imports: [CommonModule, RouterModule, MonthlyGridComponent, TopicEditorComponent, TagManagementComponent],
  templateUrl: './monthly-calendar-page.component.html',
  styleUrl: './monthly-calendar-page.component.css'
})
export class MonthlyCalendarPageComponent implements OnInit {
  readonly service = inject(MonthlyCalendarService);

  readonly showMonthPicker = signal(false);
  readonly showTagPicker = signal(false);
  readonly showTagManager = signal(false);
  readonly showTopicEditor = signal(false);
  readonly editingTopic = signal<Memento | null>(null);

  // Filters
  readonly selectedTagIds = signal<number[]>([]);
  readonly includeChildren = signal(true);
  readonly showTimeline = signal(false);
  readonly isVerticalView = signal(false);
  readonly forceShowTooltips = signal(false);
  readonly showAchieved = signal(false);

  readonly topics = this.service.topicRows;
  readonly childrenByParent = this.service.childrenByParent;
  readonly selectedMonths = this.service.selectedMonths;
  readonly displayMode = this.service.displayMode;
  readonly today = signal(new Date());

  constructor() {
    effect(() => {
      this.service.loadMementos({
        year: 2026,
        tagIds: this.selectedTagIds().length > 0 ? this.selectedTagIds() : undefined,
        includeChildren: this.includeChildren(),
        showAchieved: this.showAchieved()
      });
    });
  }

  ngOnInit() {
    // Initial tag load is enough, mementos are loaded by effect
    this.service.loadTags();
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

  onTagToggle(tagId: number) {
    this.selectedTagIds.update(ids =>
      ids.includes(tagId) ? ids.filter(id => id !== tagId) : [...ids, tagId]
    );
  }

  toggleIncludeChildren() {
    this.includeChildren.update(v => !v);
  }

  toggleShowTimeline() {
    this.showTimeline.update(v => !v);
  }

  toggleShowAchieved() {
    this.showAchieved.update(v => !v);
  }

  toggleAxes() {
    this.isVerticalView.update(v => !v);
  }

  toggleForceShowTooltips() {
    this.forceShowTooltips.update(v => !v);
  }

  onManageTags() {
    this.showTagPicker.set(false);
    this.showTagManager.set(true);
  }

  onDisplayModeChange(mode: DisplayMode) {
    this.service.setDisplayMode(mode);
  }

  onModeSelectChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.onDisplayModeChange(select.value as DisplayMode);
  }

  onAddTopic() {
    this.editingTopic.set(null);
    this.showTopicEditor.set(true);
  }

  onTopicClick(topic: Memento) {
    this.editingTopic.set(topic);
    this.showTopicEditor.set(true);
  }

  onEditorSave(topic: Memento) {
    if (topic.id > 0) {
      this.service.updateTopic(topic);
    } else {
      this.service.addTopic(topic);
    }
    this.showTopicEditor.set(false);
  }

  onEditorDelete(id: number) {
    this.service.deleteTopic(id);
    this.showTopicEditor.set(false);
  }

  onEditorCancel() {
    this.showTopicEditor.set(false);
  }

  onPhaseClick(phase: Memento) {
    console.log('Phase clicked:', phase);
  }
}
