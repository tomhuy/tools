import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MoodEntry, MOODS, ACTIVITY_TAGS, MoodConfig } from '../../../models/weekly-tracker.model';
import { MoodTrackerService } from '../weekly-tracker.service';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';

@Component({
  selector: 'app-entry-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './entry-editor.component.html',
  styleUrl: './entry-editor.component.css'
})
export class MoodEntryEditorComponent {
  @Input({ required: true }) entry!: MoodEntry;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<MoodEntry>();
  @Output() delete = new EventEmitter<string>();

  trackerService = inject(MoodTrackerService);
  
  moods = MOODS;
  tags = ACTIVITY_TAGS;

  get formattedDate() {
    return format(this.entry.date, 'EEEE, dd/MM', { locale: vi });
  }

  get formattedTime() {
    return format(this.entry.date, 'HH:mm');
  }

  toggleTag(tagId: string) {
    if (this.entry.tags.includes(tagId)) {
      this.entry.tags = this.entry.tags.filter((id: string) => id !== tagId);
    } else {
      this.entry.tags.push(tagId);
    }
  }

  onSave() {
    // Basic validation or preprocessing if needed
    this.trackerService.saveEntry(this.entry);
    this.save.emit(this.entry);
  }

  onDelete() {
    if (this.entry.id) {
      this.trackerService.deleteEntry(this.entry.id);
      this.delete.emit(this.entry.id);
    }
  }

  onCancel() {
    this.close.emit();
  }
}
