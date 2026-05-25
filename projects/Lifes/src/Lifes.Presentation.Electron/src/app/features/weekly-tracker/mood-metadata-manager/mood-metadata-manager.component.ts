import { Component, inject, signal, OnInit, Output, EventEmitter, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MoodMetadataApiService } from '../services/mood-metadata-api.service';
import { MoodMetadataDefinition } from '../../../models/weekly-tracker.model';

@Component({
  selector: 'app-mood-metadata-manager',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './mood-metadata-manager.component.html',
  styleUrl: './mood-metadata-manager.component.css'
})
export class MoodMetadataManagerComponent implements OnInit {
  private readonly apiService = inject(MoodMetadataApiService);
  private readonly destroyRef = inject(DestroyRef);

  @Output() close = new EventEmitter<void>();
  @Output() changed = new EventEmitter<void>();

  fields = signal<MoodMetadataDefinition[]>([]);
  selectedField = signal<MoodMetadataDefinition | null>(null);
  isEditMode = signal(false);

  // Form Model
  formKey = '';
  formLabelDisplay = '';
  formDescription = '';
  formInputType = 'text';
  formEnabled = true;
  formOptionsText = ''; // options joined by comma

  inputTypes = [
    { value: 'text', label: 'Văn bản (Text)' },
    { value: 'number', label: 'Số (Number)' },
    { value: 'select', label: 'Lựa chọn (Select)' },
    { value: 'checkbox', label: 'Đánh dấu (Checkbox)' },
    { value: 'textarea', label: 'Khung văn bản (Textarea)' },
    { value: 'date', label: 'Ngày (Date)' },
    { value: 'time', label: 'Giờ (Time)' },
    { value: 'datetime', label: 'Ngày & Giờ (DateTime)' },
    { value: 'duration', label: 'Khoảng thời gian (Duration)' }
  ];

  ngOnInit() {
    this.loadFields();
  }

  loadFields() {
    this.apiService.getAll().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        this.fields.set(data);
      },
      error: (err) => console.error('Failed to load metadata fields', err)
    });
  }

  onAddNew() {
    this.selectedField.set(null);
    this.isEditMode.set(true);
    this.formKey = '';
    this.formLabelDisplay = '';
    this.formDescription = '';
    this.formInputType = 'text';
    this.formEnabled = true;
    this.formOptionsText = '';
  }

  onSelectField(field: MoodMetadataDefinition) {
    this.selectedField.set(field);
    this.isEditMode.set(true);
    this.formKey = field.key;
    this.formLabelDisplay = field.labelDisplay;
    this.formDescription = field.description ?? '';
    this.formInputType = field.inputType;
    this.formEnabled = field.enabled;
    
    if (field.options && field.options.length > 0) {
      const isJsonOptions = field.options.some(opt => opt.trim().startsWith('{') && opt.trim().endsWith('}'));
      if (isJsonOptions) {
        const parsedList = field.options.map(opt => {
          try {
            return JSON.parse(opt);
          } catch (e) {
            return opt;
          }
        });
        this.formOptionsText = JSON.stringify(parsedList, null, 2);
      } else {
        this.formOptionsText = field.options.join(', ');
      }
    } else {
      this.formOptionsText = '';
    }
  }

  slugify(text: string): string {
    return text
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/[^a-z0-9\s_]/g, '')
      .trim()
      .replace(/[\s-]+/g, '_');
  }

  onAutoGenerateKey() {
    if (!this.selectedField() && this.formLabelDisplay) {
      this.formKey = this.slugify(this.formLabelDisplay);
    }
  }

  onSave() {
    if (!this.formLabelDisplay.trim()) {
      alert('Vui lòng nhập Tên hiển thị!');
      return;
    }

    const finalKey = this.formKey.trim() || this.slugify(this.formLabelDisplay);
    if (!finalKey) {
      alert('Vui lòng nhập Key duy nhất!');
      return;
    }

    // Check duplicate key when adding new field
    if (!this.selectedField()) {
      const duplicate = this.fields().some(f => f.key.toLowerCase() === finalKey.toLowerCase());
      if (duplicate) {
        alert('Key này đã tồn tại, vui lòng chọn key khác!');
        return;
      }
    }

    // Parse options (handles JSON array or comma-separated lists)
    let options: string[] = [];
    const trimmedOptionsText = this.formOptionsText.trim();
    if (trimmedOptionsText.startsWith('[') && trimmedOptionsText.endsWith(']')) {
      try {
        const parsed = JSON.parse(trimmedOptionsText);
        if (Array.isArray(parsed)) {
          options = parsed.map(item => {
            if (typeof item === 'object' && item !== null) {
              return JSON.stringify(item);
            }
            return String(item);
          });
        }
      } catch (e) {
        // Fallback to comma split if JSON array parsing fails
      }
    }

    if (options.length === 0) {
      options = this.formOptionsText
        .split(',')
        .map(o => o.trim())
        .filter(o => o.length > 0);
    }

    const payload: MoodMetadataDefinition = {
      key: finalKey,
      labelDisplay: this.formLabelDisplay.trim(),
      description: this.formDescription.trim(),
      inputType: this.formInputType,
      enabled: this.formEnabled,
      options: options.length > 0 ? options : undefined
    };

    this.apiService.save(payload).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.loadFields();
        this.isEditMode.set(false);
        this.selectedField.set(null);
        this.changed.emit();
      },
      error: (err) => {
        console.error('Failed to save field definition', err);
        alert('Có lỗi xảy ra khi lưu trường dữ liệu!');
      }
    });
  }

  onDelete(field: MoodMetadataDefinition) {
    if (confirm(`Bạn có chắc chắn muốn xóa trường "${field.labelDisplay}"?`)) {
      this.apiService.delete(field.key).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
        next: () => {
          this.loadFields();
          this.isEditMode.set(false);
          this.selectedField.set(null);
          this.changed.emit();
        },
        error: (err) => {
          console.error('Failed to delete field definition', err);
          alert('Có lỗi xảy ra khi xóa trường dữ liệu!');
        }
      });
    }
  }

  onToggleEnabled(field: MoodMetadataDefinition, event: Event) {
    event.stopPropagation();
    const updated = { ...field, enabled: !field.enabled };
    this.apiService.save(updated).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.loadFields();
        this.changed.emit();
      },
      error: (err) => console.error('Failed to toggle enabled state', err)
    });
  }

  onCancelEdit() {
    this.isEditMode.set(false);
    this.selectedField.set(null);
  }

  onCancel() {
    this.close.emit();
  }
}
