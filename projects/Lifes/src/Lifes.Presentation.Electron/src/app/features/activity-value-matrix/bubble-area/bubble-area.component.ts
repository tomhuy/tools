import { Component, computed, inject, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivityItem } from '../models/activity-value-matrix.model';
import { ActivityValueMatrixService } from '../activity-value-matrix.service';

@Component({
  selector: 'app-bubble-area',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './bubble-area.component.html',
  styleUrl: './bubble-area.component.css',
})
export class BubbleAreaComponent {
  private svc = inject(ActivityValueMatrixService);

  readonly activities = input.required<ActivityItem[]>();
  readonly selectedId = input<string | null>(null);
  readonly bubbleClick = output<string>();

  getBubbleSize(activity: ActivityItem): number {
    return this.svc.getBubbleSize(activity);
  }

  onBubbleClick(id: string): void {
    this.bubbleClick.emit(id);
  }

  trackById(_: number, item: ActivityItem): string {
    return item.id;
  }
}
