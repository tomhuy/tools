import { inject, Injectable, signal, effect } from '@angular/core';
import { MementoService } from '../../core/services/memento.service';
import { Memento } from '../../models/memento.model';

@Injectable() // Local provider for ViewChartPage
export class ViewChartService {
  private readonly mementoService = inject(MementoService);

  // State
  readonly searchTerm = signal('');
  readonly selectedTopics = signal<Memento[]>([]);
  
  // Expose signals from MementoService
  readonly topics = this.mementoService.mementos;
  readonly isLoading = this.mementoService.isLoading;

  constructor() {
    // Automatically reload topics when search term changes
    effect(() => {
      const term = this.searchTerm();
      this.mementoService.loadMementos({ keyword: term });
    });
  }

  setSearchTerm(term: string) {
    this.searchTerm.set(term);
  }

  toggleTopicSelection(topic: Memento) {
    const currentSelected = this.selectedTopics();
    const isSelected = currentSelected.some(t => t.id === topic.id);

    if (isSelected) {
      this.selectedTopics.set(currentSelected.filter(t => t.id !== topic.id));
    } else {
      this.selectedTopics.set([...currentSelected, topic]);
    }
  }

  isSelected(topicId: number): boolean {
    return this.selectedTopics().some(t => t.id === topicId);
  }

  removeTopic(topicId: number) {
    this.selectedTopics.update(list => list.filter(t => t.id !== topicId));
  }
}
