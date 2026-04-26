import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Memento } from '../../../models/memento.model';
import { ChartSeries } from '../../../models/chart.model';
import { ChartVisualizerComponent } from '../chart-visualizer/chart-visualizer.component';
import { D3SampleComponent } from '../d3-sample/d3-sample.component';

@Component({
  selector: 'app-chart-container',
  standalone: true,
  imports: [CommonModule, ChartVisualizerComponent, D3SampleComponent],
  templateUrl: './chart-container.component.html',
  styleUrl: './chart-container.component.css'
})
export class ChartContainerComponent {
  @Input() selectedTopics: Memento[] = [];
  @Input() chartData: ChartSeries[] = [];
  @Input() monthLabel: string = '';
  
  @Output() removeTopic = new EventEmitter<number>();
  @Output() configureTopic = new EventEmitter<number>();
  @Output() render = new EventEmitter<void>();

  onRemove(topicId: number, event: Event) {
    event.stopPropagation();
    this.removeTopic.emit(topicId);
  }

  onConfigure(topicId: number) {
    this.configureTopic.emit(topicId);
  }

  onRender() {
    this.render.emit();
  }
}
