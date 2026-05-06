import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SprintBoardService } from './sprint-board.service';
import { UserService } from '../users/user.service';
import { SprintFeature, SprintTask } from '../../models/sprint-board.model';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-sprint-board',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sprint-board.component.html',
  styleUrl: './sprint-board.component.css'
})
export class SprintBoardComponent implements OnInit {
  private readonly boardService = inject(SprintBoardService);
  private readonly userService = inject(UserService);

  // State
  readonly epics = this.boardService.epics;
  readonly users = this.userService.users;
  readonly showDone = signal<boolean>(true);
  readonly activeHighlight = signal<string | null>(null);
  readonly showArchived = signal<boolean>(false);

  // Modal State
  readonly isEpicModalOpen = signal(false);
  readonly isUserModalOpen = signal(false);
  readonly editingEpic = signal<SprintFeature | null>(null);
  readonly newSubtaskName = signal('');

  // Computed
  readonly activeEpics = computed(() => 
    this.epics().filter(e => !e.archived && e.status === 'progress')
  );

  readonly backlogEpics = computed(() => 
    this.epics().filter(e => !e.archived && e.status === 'backlog')
  );

  readonly archivedEpics = computed(() => 
    this.epics().filter(e => e.archived)
  );

  readonly summaryData = computed(() => {
    return this.users().map(u => {
      const tasks = this.epics().filter(e => !e.archived).flatMap(f => f.tasks).filter(t => t.assigneeId === u.id);
      const done = tasks.filter(t => t.done).length;
      return {
        ...u,
        total: tasks.length,
        done,
        active: tasks.length - done
      };
    });
  });

  readonly boardColumns = computed(() => {
    const cols: { id: string, color?: string, isUser: boolean }[] = [
      { id: 'pre', isUser: false }
    ];
    this.users().forEach(u => cols.push({ id: u.id, color: u.color, isUser: true }));
    return cols;
  });

  ngOnInit(): void {
    this.userService.loadUsers().subscribe();
    this.boardService.loadBoard().subscribe();
  }

  // Actions
  toggleDone(): void {
    this.showDone.update(v => !v);
  }

  toggleArchivedView(): void {
    this.showArchived.update(v => !v);
  }

  toggleHighlight(userId: string): void {
    this.activeHighlight.set(this.activeHighlight() === userId ? null : userId);
  }

  onTaskClick(epicId: string, taskId: string): void {
    this.boardService.toggleTaskDone(epicId, taskId);
  }

  onReassign(epicId: string, taskId: string, assigneeId: string): void {
    this.boardService.moveTask(taskId, epicId, epicId, assigneeId);
  }

  // Epic Management
  openAddEpic(): void {
    const newEpic: SprintFeature = {
      id: 'e' + Date.now(),
      name: 'New Epic',
      color: 'blue',
      archived: false,
      status: 'backlog',
      tasks: []
    };
    this.editingEpic.set(newEpic);
    this.isEpicModalOpen.set(true);
  }

  openEditEpic(epic: SprintFeature): void {
    this.editingEpic.set(JSON.parse(JSON.stringify(epic))); // Deep clone
    this.isEpicModalOpen.set(true);
  }

  closeEpicModal(): void {
    this.isEpicModalOpen.set(false);
    this.editingEpic.set(null);
  }

  saveEpic(): void {
    const epic = this.editingEpic();
    if (!epic) return;

    const current = this.epics();
    const index = current.findIndex(e => e.id === epic.id);
    
    if (index > -1) {
      current[index] = epic;
    } else {
      current.push(epic);
    }

    this.boardService.saveBoard(current).subscribe(() => this.closeEpicModal());
  }

  deleteEpic(id: string): void {
    if (!confirm('Xóa Epic này?')) return;
    const current = this.epics().filter(e => e.id !== id);
    this.boardService.saveBoard(current).subscribe(() => this.closeEpicModal());
  }

  // Subtask Management inside Modal
  addSubtask(): void {
    const name = this.newSubtaskName().trim();
    const epic = this.editingEpic();
    if (!name || !epic) return;

    const newTask: SprintTask = {
      id: 't' + Date.now(),
      label: (epic.tasks.length + 1).toString(),
      name: name,
      assigneeId: 'pre',
      done: false
    };

    epic.tasks.push(newTask);
    this.newSubtaskName.set('');
  }

  removeSubtask(taskId: string): void {
    const epic = this.editingEpic();
    if (epic) {
      epic.tasks = epic.tasks.filter(t => t.id !== taskId);
    }
  }

  toggleSubtaskPriority(task: SprintTask): void {
    task.isTopPriority = !task.isTopPriority;
  }

  // User Management
  openUserModal(): void {
    this.isUserModalOpen.set(true);
  }

  closeUserModal(): void {
    this.isUserModalOpen.set(false);
  }

  addUser(): void {
    const newUser: User = {
      id: 'u' + Date.now(),
      name: 'New Member',
      initials: 'NM',
      color: 'blue'
    };
    this.userService.saveUser(newUser).subscribe();
  }

  removeUser(id: string): void {
    if (!confirm('Xóa thành viên này?')) return;
    this.userService.deleteUser(id).subscribe();
  }

  updateUser(user: User): void {
    this.userService.saveUser(user).subscribe();
  }

  getAssigneeColor(userId: string): string {
    if (userId === 'pre') return 'red';
    const user = this.users().find(u => u.id === userId);
    return user?.color || 'gray';
  }

  getAssigneeName(userId: string): string {
    const user = this.users().find(u => u.id === userId);
    return user?.name || '?';
  }

  cycleAssignee(task: SprintTask): void {
    const order = ['pre', ...this.users().map(u => u.id)];
    const i = order.indexOf(task.assigneeId);
    task.assigneeId = order[(i + 1) % order.length];
  }

  // Drag & Drop
  private dragInfo: { epicId: string; taskId: string } | null = null;

  onDragStart(event: DragEvent, epicId: string, taskId: string): void {
    this.dragInfo = { epicId, taskId };
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
    }
    const target = event.target as HTMLElement;
    setTimeout(() => target.classList.add('dragging'), 0);
  }

  onDragEnd(event: DragEvent): void {
    (event.target as HTMLElement).classList.remove('dragging');
    this.dragInfo = null;
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    (event.currentTarget as HTMLElement).classList.add('drag-over');
  }

  onDragLeave(event: DragEvent): void {
    (event.currentTarget as HTMLElement).classList.remove('drag-over');
  }

  onDrop(event: DragEvent, epicId: string, assigneeId: string): void {
    event.preventDefault();
    (event.currentTarget as HTMLElement).classList.remove('drag-over');
    if (this.dragInfo) {
      this.boardService.moveTask(this.dragInfo.taskId, this.dragInfo.epicId, epicId, assigneeId);
    }
  }

  // Helper for sorting tasks (priority first)
  getSortedTasks(tasks: SprintTask[]): SprintTask[] {
    return [...tasks].sort((a, b) => (b.isTopPriority ? 1 : 0) - (a.isTopPriority ? 1 : 0));
  }

  // Common Palette
  readonly colors = ['blue', 'green', 'orange', 'purple', 'pink', 'teal', 'indigo', 'red'];
}
