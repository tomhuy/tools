import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ViewChartService } from '../view-chart.service';

@Component({
  selector: 'app-topic-search-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './topic-search-list.component.html',
  styleUrl: './topic-search-list.component.css'
})
export class TopicSearchListComponent {
  protected readonly service = inject(ViewChartService);
}
