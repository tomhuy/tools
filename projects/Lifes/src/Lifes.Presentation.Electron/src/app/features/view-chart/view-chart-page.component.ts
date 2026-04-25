import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ViewChartService } from './view-chart.service';
import { MementoService } from '../../core/services/memento.service';
import { TopicSearchListComponent } from './topic-search-list/topic-search-list.component';
import { ChartContainerComponent } from './chart-container/chart-container.component';

@Component({
  selector: 'app-view-chart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TopicSearchListComponent, ChartContainerComponent],
  providers: [MementoService, ViewChartService], // LOCAL SCOPE
  templateUrl: './view-chart-page.component.html',
  styleUrl: './view-chart-page.component.css'
})
export class ViewChartPageComponent {
  protected readonly service = inject(ViewChartService);
}
