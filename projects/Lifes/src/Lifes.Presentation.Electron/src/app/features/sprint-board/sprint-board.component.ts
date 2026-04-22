import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SprintBoardService } from './sprint-board.service';
import { SprintTask, Person } from '../../models/sprint-board.model';

@Component({
  selector: 'app-sprint-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sprint-board.component.html',
  styleUrl: './sprint-board.component.css'
})
export class SprintBoardComponent {
  private readonly boardService = inject(SprintBoardService);

  // State
  readonly boardData = this.boardService.boardData;
  readonly people = this.boardService.people;
  readonly showDone = signal<boolean>(true);
  readonly activeHighlight = signal<string | null>(null);

  // Computed
  readonly summaryData = computed(() => {
    return this.people().map(p => {
      const tasks = this.boardData().features.flatMap(f => f.tasks).filter(t => t.person === p.id);
      const done = tasks.filter(t => t.done).length;
      return {
        ...p,
        total: tasks.length,
        done,
        active: tasks.length - done
      };
    });
  });

  // Drag & Drop tracking
  private dragInfo: { featureId: string; taskId: string; person: string } | null = null;

  toggleDone(event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    this.showDone.set(checkbox.checked);
  }

  toggleHighlight(personId: string): void {
    if (this.activeHighlight() === personId) {
      this.activeHighlight.set(null);
    } else {
      this.activeHighlight.set(personId);
    }
  }

  onTaskClick(featureId: string, taskId: string): void {
    this.boardService.toggleTaskDone(featureId, taskId);
  }

  onAddTask(featureId: string): void {
    const name = prompt('Tên task mới:');
    if (!name) return;
    
    const label = prompt('ID (ví dụ: 1.2):') || '0.0';
    
    const personList = ['pre', ...this.people().map(p => p.id)];
    const personPrompt = `Ai phụ trách?\n0: Làm trước\n${this.people().map((p, i) => `${i + 1}: ${p.label}`).join('\n')}\nNhập số:`;
    const personIdx = parseInt(prompt(personPrompt) || '0');
    const person = personList[personIdx] || 'pre';

    this.boardService.addTask(featureId, { name, label, person });
  }

  onAddFeature(): void {
    const name = prompt('Tên feature mới:');
    if (name) {
      this.boardService.addFeature(name);
    }
  }

  // Native Drag & Drop Handlers
  onDragStart(event: DragEvent, featureId: string, taskId: string, person: string): void {
    this.dragInfo = { featureId, taskId, person };
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
    }
    // Add visual class to target
    const target = event.target as HTMLElement;
    setTimeout(() => target.classList.add('dragging'), 0);
  }

  onDragEnd(event: DragEvent): void {
    const target = event.target as HTMLElement;
    target.classList.remove('dragging');
    this.dragInfo = null;
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    const cell = event.currentTarget as HTMLElement;
    cell.classList.add('drag-over');
  }

  onDragLeave(event: DragEvent): void {
    const cell = event.currentTarget as HTMLElement;
    cell.classList.remove('drag-over');
  }

  onDrop(event: DragEvent, featureId: string, personId: string): void {
    event.preventDefault();
    const cell = event.currentTarget as HTMLElement;
    cell.classList.remove('drag-over');

    if (this.dragInfo) {
      this.boardService.moveTask(this.dragInfo.taskId, this.dragInfo.featureId, featureId, personId);
    }
  }

  onMoveHintClick(featureId: string, taskId: string, personId: string): void {
    this.boardService.moveTask(taskId, featureId, featureId, personId);
  }
}
