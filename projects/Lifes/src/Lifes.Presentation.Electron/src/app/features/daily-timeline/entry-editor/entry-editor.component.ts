import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ENERGY_LEVELS, EnergyGrade, DailyEntry } from '../../../models/daily-timeline.model';

@Component({
  selector: 'app-entry-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './entry-editor.component.html',
  styleUrl: './entry-editor.component.css'
})
export class EntryEditorComponent implements OnInit {
  @Input() entry!: DailyEntry;
  @Output() save = new EventEmitter<DailyEntry>();
  @Output() delete = new EventEmitter<number>();
  @Output() close = new EventEmitter<void>();

  energyLevels = ENERGY_LEVELS;
  availableTags = [
    'Làm việc một mình', 'Họp', 'Gặp bạn bè', 
    'Ở cùng gia đình', 'Tập thể dục', 
    'Đọc sách', 'Nghe nhạc', 'Ăn uống', 
    'Di chuyển', 'Nghỉ ngơi', 'Coding', 'Sáng tạo'
  ];

  selectedGrade?: EnergyGrade;
  selectedTags: string[] = [];
  note: string = '';

  ngOnInit() {
    this.selectedGrade = this.entry.energyGrade;
    this.selectedTags = [...this.entry.tags];
    this.note = this.entry.note || '';
  }

  formatHour(hour: number): string {
    return `${hour.toString().padStart(2, '0')}:00`;
  }

  toggleTag(tag: string) {
    if (this.selectedTags.includes(tag)) {
      this.selectedTags = this.selectedTags.filter(t => t !== tag);
    } else {
      this.selectedTags.push(tag);
    }
  }

  onSave() {
    this.save.emit({
      ...this.entry,
      energyGrade: this.selectedGrade,
      tags: this.selectedTags,
      note: this.note
    });
  }

  onDelete() {
    this.delete.emit(this.entry.hour);
  }

  onBackdropClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.close.emit();
    }
  }
}
