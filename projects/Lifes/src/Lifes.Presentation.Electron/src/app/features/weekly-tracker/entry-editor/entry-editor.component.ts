import { Component, Input, Output, EventEmitter, inject, OnInit, signal, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MoodEntry, MOODS, ACTIVITY_TAGS, MoodConfig, MoodMetadataDefinition } from '../../../models/weekly-tracker.model';
import { MoodTrackerService } from '../weekly-tracker.service';
import { MoodMetadataApiService } from '../services/mood-metadata-api.service';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';

@Component({
  selector: 'app-entry-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './entry-editor.component.html',
  styleUrl: './entry-editor.component.css'
})
export class MoodEntryEditorComponent implements OnInit {
  @Input({ required: true }) entry!: MoodEntry;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<MoodEntry>();
  @Output() delete = new EventEmitter<string>();

  trackerService = inject(MoodTrackerService);
  private readonly metadataApi = inject(MoodMetadataApiService);
  private readonly destroyRef = inject(DestroyRef);
  
  moods = MOODS;
  tags = ACTIVITY_TAGS;
  activeFields = signal<MoodMetadataDefinition[]>([]);

  ngOnInit() {
    if (!this.entry.metadata) {
      this.entry.metadata = {};
    }

    this.metadataApi.getAll().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        // Lọc các trường được kích hoạt và sắp xếp theo thứ tự hiển thị
        const enabledFields = data
          .filter(f => f.enabled)
          .sort((a, b) => (a.order || 0) - (b.order || 0));
        this.activeFields.set(enabledFields);

        // Khởi tạo giá trị mặc định cho form
        enabledFields.forEach(field => {
          if (this.entry.metadata && this.entry.metadata[field.key] === undefined) {
            if (field.inputType === 'checkbox') {
              this.entry.metadata[field.key] = false;
            } else if (field.inputType === 'select') {
              const firstOpt = field.options && field.options.length > 0 ? field.options[0] : '';
              this.entry.metadata[field.key] = this.parseOption(firstOpt).value;
            } else {
              this.entry.metadata[field.key] = '';
            }
          }
        });
      },
      error: (err) => console.error('Failed to load active metadata fields', err)
    });
  }

  parseOption(opt: string): { label: string; value: string } {
    if (!opt) return { label: '', value: '' };
    
    const trimmed = opt.trim();
    if (trimmed.startsWith('{') && trimmed.endsWith('}')) {
      try {
        const parsed = JSON.parse(trimmed);
        if (parsed.label !== undefined && parsed.value !== undefined) {
          return { label: String(parsed.label), value: String(parsed.value) };
        }
      } catch (e) {
        // Fallback to standard parsing if JSON parse fails
      }
    }

    const index = opt.indexOf(':');
    if (index !== -1) {
      return {
        label: opt.substring(0, index).trim(),
        value: opt.substring(index + 1).trim()
      };
    }
    return { label: opt.trim(), value: opt.trim() };
  }

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
