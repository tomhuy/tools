import { Injectable, signal } from '@angular/core';
import { SprintBoardData, SprintFeature, SprintTask, Person } from '../../models/sprint-board.model';

@Injectable({
  providedIn: 'root'
})
export class SprintBoardService {
  readonly people = signal<Person[]>([
    { id: 'huy', label: 'Huy', initials: 'HY', color: 'blue' },
    { id: 'tuan', label: 'Tuấn', initials: 'TN', color: 'green' },
    { id: 'bang', label: 'Bằng', initials: 'BG', color: 'orange' },
    { id: 'hoa', label: 'Hòa', initials: 'HÒ', color: 'purple' }
  ]);

  readonly boardData = signal<SprintBoardData>({
    features: [
      {
        id: 'f1', name: 'Refactor\nPS Mode', accent: 'blue',
        tasks: [
          { id: 't1', label: '01', name: 'lấy file cho design', person: 'pre', done: false },
          { id: 't2', label: '02', name: 'investigate', person: 'bang', done: false },
        ]
      },
      {
        id: 'f2', name: 'Add validation\ndataloader', accent: 'green',
        tasks: [
          { id: 't3', label: '1.1', name: 'Check the feedback and split item', person: 'pre', done: false },
        ]
      },
      {
        id: 'f3', name: 'Custom text', accent: 'orange',
        tasks: [
          { id: 't4', label: '2.1', name: 'implement', person: 'hoa', done: true },
          { id: 't5', label: '2.1', name: 'Code review', person: 'tuan', done: false },
          { id: 't6', label: '2.2', name: 'Code review', person: 'bang', done: false },
        ]
      },
      {
        id: 'f4', name: 'IXL Time spent', accent: 'purple',
        tasks: [
          { id: 't7', label: '3.1', name: 'investigate', person: 'tuan', done: false },
          { id: 't8', label: '3.2', name: 'discuss with huy', person: 'huy', done: false },
          { id: 't9', label: '3.3', name: 'implement', person: 'pre', done: false },
        ]
      }
    ]
  });

  private nextTaskId = 100;

  toggleTaskDone(featureId: string, taskId: string): void {
    this.boardData.update(data => {
      const feature = data.features.find(f => f.id === featureId);
      if (feature) {
        const task = feature.tasks.find(t => t.id === taskId);
        if (task) {
          task.done = !task.done;
        }
      }
      return { ...data };
    });
  }

  moveTask(taskId: string, fromFeatureId: string, toFeatureId: string, toPerson: string): void {
    this.boardData.update(data => {
      const fromFeature = data.features.find(f => f.id === fromFeatureId);
      if (!fromFeature) return data;

      const taskIndex = fromFeature.tasks.findIndex(t => t.id === taskId);
      if (taskIndex === -1) return data;

      const [task] = fromFeature.tasks.splice(taskIndex, 1);
      task.person = toPerson;

      const toFeature = data.features.find(f => f.id === toFeatureId);
      if (toFeature) {
        toFeature.tasks.push(task);
      }

      return { ...data };
    });
  }

  addTask(featureId: string, task: Partial<SprintTask>): void {
    this.boardData.update(data => {
      const feature = data.features.find(f => f.id === featureId);
      if (feature) {
        const newTask: SprintTask = {
          id: `t${this.nextTaskId++}`,
          label: task.label || '0.0',
          name: task.name || 'New Task',
          person: task.person || 'pre',
          done: false
        };
        feature.tasks.push(newTask);
      }
      return { ...data };
    });
  }

  addFeature(name: string): void {
    this.boardData.update(data => {
      const accents = ['blue', 'green', 'orange', 'purple'];
      const newFeature: SprintFeature = {
        id: `f${data.features.length + 1}`,
        name: name,
        accent: accents[data.features.length % accents.length],
        tasks: []
      };
      data.features.push(newFeature);
      return { ...data };
    });
  }
}
