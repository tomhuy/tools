import { inject, Injectable, signal, effect } from '@angular/core';
import { MementoService } from '../../core/services/memento.service';
import { Memento } from '../../models/memento.model';
import { ChartSetting } from '../../models/chart.model';

@Injectable() // Local provider for ViewChartPage
export class ViewChartService {
  private readonly mementoService = inject(MementoService);

  // State
  readonly searchTerm = signal('');
  readonly selectedTopics = signal<Memento[]>([]);
  readonly selectedMonth = signal(new Date().getMonth() + 1);
  readonly selectedYear = signal(new Date().getFullYear());
  
  // Topic configurations (Map: topicId -> setting)
  readonly topicSettings = signal<Map<number, ChartSetting>>(new Map());
  
  // Detailed data (Map: topicId -> Memento with children)
  readonly topicDataMap = signal<Map<number, Memento>>(new Map());

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
      // Also remove settings and data
      this.topicSettings.update(map => {
        const newMap = new Map(map);
        newMap.delete(topic.id);
        return newMap;
      });
      this.topicDataMap.update(map => {
        const newMap = new Map(map);
        newMap.delete(topic.id);
        return newMap;
      });
    } else {
      this.selectedTopics.set([...currentSelected, topic]);
      this.loadTopicData(topic.id);
    }
  }

  loadTopicData(topicId: number) {
    this.mementoService.getTopicDetails(topicId, this.selectedMonth(), this.selectedYear())
      .subscribe((topicWithChildren: Memento | null) => {
        if (topicWithChildren) {
          this.topicDataMap.update(map => {
            const newMap = new Map(map);
            newMap.set(topicId, topicWithChildren);
            return newMap;
          });
        }
      });
  }

  saveTopicSetting(topicId: number, setting: ChartSetting) {
    this.topicSettings.update(map => {
      const newMap = new Map(map);
      newMap.set(topicId, setting);
      return newMap;
    });
  }

  getTopicSetting(topicId: number): ChartSetting | undefined {
    return this.topicSettings().get(topicId);
  }

  isSelected(topicId: number): boolean {
    return this.selectedTopics().some(t => t.id === topicId);
  }

  removeTopic(topicId: number) {
    this.selectedTopics.update(list => list.filter(t => t.id !== topicId));
    this.topicSettings.update(map => {
      const newMap = new Map(map);
      newMap.delete(topicId);
      return newMap;
    });
    this.topicDataMap.update(map => {
      const newMap = new Map(map);
      newMap.delete(topicId);
      return newMap;
    });
  }
}
