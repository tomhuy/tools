import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Memento } from '../../../models/memento.model';
import { Tag } from '../../../models/tag.model';

export type SortField = 'title' | 'startDate' | 'endDate' | 'order';
export type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-memento-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './memento-table.component.html',
  styleUrl: './memento-table.component.css'
})
export class MementoTableComponent {
  data = input.required<Memento[]>();
  tags = input<Tag[]>([]);
  
  updateOrder = output<{ memento: Memento, order: number }>();
  edit = output<Memento>();
  delete = output<number>();

  sortField = signal<SortField>('order');
  sortDirection = signal<SortDirection>('asc');

  sortedData = computed(() => {
    const field = this.sortField();
    const direction = this.sortDirection();
    const items = [...this.data()];

    return items.sort((a, b) => {
      let valA: any = a[field as keyof Memento];
      let valB: any = b[field as keyof Memento];

      // Special handling for dates
      if (field === 'startDate' || field === 'endDate') {
        valA = new Date(valA).getTime();
        valB = new Date(valB).getTime();
      }

      if (valA < valB) return direction === 'asc' ? -1 : 1;
      if (valA > valB) return direction === 'asc' ? 1 : -1;
      return 0;
    });
  });

  toggleSort(field: SortField) {
    if (this.sortField() === field) {
      this.sortDirection.update(d => d === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortField.set(field);
      this.sortDirection.set('asc');
    }
  }

  getTagForId(tagId: number): Tag | undefined {
    return this.tags().find(t => t.id === tagId);
  }

  onOrderBlur(memento: Memento, event: any) {
    const newOrder = parseInt(event.target.value, 10);
    if (!isNaN(newOrder) && newOrder !== memento.order) {
      this.updateOrder.emit({ memento, order: newOrder });
    }
  }

  onOrderKeydown(memento: Memento, event: KeyboardEvent) {
    if (event.key === 'Enter') {
      (event.target as HTMLInputElement).blur();
    }
  }
}
