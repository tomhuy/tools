import { Component, EventEmitter, Input, Output, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Memento } from '../../../models/memento.model';
import { MonthlyCalendarService } from '../monthly-calendar.service';
import { STANDARD_COLOR_PALETTE } from '../../../utils/color-utils';

@Component({
  selector: 'app-topic-editor',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './topic-editor.component.html',
  styleUrl: './topic-editor.component.css'
})
export class TopicEditorComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly calendarService = inject(MonthlyCalendarService);

  @Input() topic: Memento | null = null;
  @Output() save = new EventEmitter<Memento>();
  @Output() cancel = new EventEmitter<void>();
  @Output() delete = new EventEmitter<number>();

  topicForm!: FormGroup;
  readonly tags = this.calendarService.tags;
  
  readonly isEditMode = computed(() => this.topic !== null);

  readonly presetColors = STANDARD_COLOR_PALETTE;

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    const today = new Date().toISOString().split('T')[0];
    
    this.topicForm = this.fb.group({
      id: [this.topic?.id ?? 0],
      title: [this.topic?.title ?? '', [Validators.required]],
      startDate: [this.topic?.startDate ?? today, [Validators.required]],
      endDate: [this.topic?.endDate ?? today, [Validators.required]],
      order: [this.topic?.order ?? 0],
      color: [this.topic?.color ?? this.presetColors[0]],
      tagId: [this.topic?.tagId ?? null],
      parentId: [null] // Always null for Topics
    }, { validators: this.dateRangeValidator });
  }

  private dateRangeValidator(group: FormGroup): { [key: string]: any } | null {
    const start = group.get('startDate')?.value;
    const end = group.get('endDate')?.value;
    return start && end && start > end ? { dateRangeInvalid: true } : null;
  }

  isFieldInvalid(field: string): boolean {
    const control = this.topicForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onSubmit(): void {
    if (this.topicForm.valid) {
      this.save.emit(this.topicForm.value);
    } else {
      this.topicForm.markAllAsTouched();
    }
  }

  onDelete(): void {
    if (this.topic && confirm('Bạn có chắc chắn muốn xóa chủ đề này? Tất cả các giai đoạn con cũng sẽ bị xóa.')) {
      this.delete.emit(this.topic.id);
    }
  }
}
