import { Component, inject, signal, computed, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TagService } from '../monthly-calendar/tag.service';
import { Memento } from '../../models/memento.model';
import { MementoTableComponent } from './memento-table/memento-table.component';
import { TopicEditorComponent } from '../monthly-calendar/topic-editor/topic-editor.component';
import { MementoManagementService } from './memento-management.service';

@Component({
  selector: 'app-memento-management',
  standalone: true,
  imports: [CommonModule, FormsModule, MementoTableComponent, TopicEditorComponent],
  templateUrl: './memento-management.component.html',
  styleUrl: './memento-management.component.css'
})
export class MementoManagementComponent implements OnInit {
  private readonly managementService = inject(MementoManagementService);
  private readonly tagService = inject(TagService);

  readonly tags = this.tagService.tags;
  readonly mementos = this.managementService.mementos;
  readonly isLoading = this.managementService.isLoading;

  // Filters state
  readonly keyword = signal('');
  readonly startDate = signal('');
  readonly endDate = signal('');
  readonly selectedTagIds = signal<number[]>([]);

  readonly showEditor = signal(false);
  readonly editingTopic = signal<Memento | null>(null);

  constructor() {
    // Auto-reload when filters change (with debouncing if needed, but simple for now)
    effect(() => {
      this.managementService.loadMementos({
        keyword: this.keyword(),
        startDate: this.startDate(),
        endDate: this.endDate(),
        tagIds: this.selectedTagIds()
      });
    });
  }

  ngOnInit(): void {
    // Initial load is handled by the effect for mementos
    this.tagService.loadTags();
  }

  toggleTagFilter(tagId: number) {
    this.selectedTagIds.update(ids => 
      ids.includes(tagId) 
        ? ids.filter(id => id !== tagId) 
        : [...ids, tagId]
    );
  }

  clearFilters() {
    this.keyword.set('');
    this.startDate.set('');
    this.endDate.set('');
    this.selectedTagIds.set([]);
  }

  onUpdateOrder(event: { memento: Memento, order: number }) {
    const updated = { ...event.memento, order: event.order };
    this.managementService.saveTopic(updated).subscribe({
      next: saved => this.managementService.updateMementoLocal(saved),
      error: err => alert(err.message ?? 'Failed to update order')
    });
  }

  onAddTopic() {
    this.editingTopic.set(null);
    this.showEditor.set(true);
  }

  onEditTopic(memento: Memento) {
    this.editingTopic.set(memento);
    this.showEditor.set(true);
  }

  onSaveTopic(memento: Memento) {
    this.managementService.saveTopic(memento).subscribe({
      next: saved => {
        if (memento.id === 0) {
          this.managementService.addMementoLocal(saved);
        } else {
          this.managementService.updateMementoLocal(saved);
        }
        this.closeEditor();
      },
      error: err => alert(err.message ?? 'Failed to save topic')
    });
  }

  onDeleteTopic(id: number) {
    if (confirm('Are you sure you want to delete this topic and all its children?')) {
      this.managementService.deleteTopic(id).subscribe({
        next: () => {
          this.managementService.removeMementoLocal(id);
          this.closeEditor();
        },
        error: err => alert(err.message ?? 'Failed to delete topic')
      });
    }
  }

  closeEditor() {
    this.showEditor.set(false);
    this.editingTopic.set(null);
  }
}
