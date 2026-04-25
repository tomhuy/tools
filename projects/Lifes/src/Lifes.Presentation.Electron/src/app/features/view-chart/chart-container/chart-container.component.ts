import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Memento } from '../../../models/memento.model';

@Component({
  selector: 'app-chart-container',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chart-container.component.html',
  styleUrl: './chart-container.component.css'
})
export class ChartContainerComponent {
  @Input() selectedTopics: Memento[] = [];
  @Output() removeTopic = new EventEmitter<number>();

  onRemove(topicId: number) {
    this.removeTopic.emit(topicId);
  }
}
