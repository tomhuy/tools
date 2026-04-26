import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ViewChartService } from './view-chart.service';
import { MementoService } from '../../core/services/memento.service';
import { TopicSearchListComponent } from './topic-search-list/topic-search-list.component';
import { ChartContainerComponent } from './chart-container/chart-container.component';
import { TopicConfigPopupComponent } from './topic-config-popup/topic-config-popup.component';
import { Memento } from '../../models/memento.model';
import { ChartSetting, ChartSeries, ChartDataPoint } from '../../models/chart.model';

@Component({
  selector: 'app-view-chart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TopicSearchListComponent, ChartContainerComponent, TopicConfigPopupComponent],
  providers: [MementoService, ViewChartService], // LOCAL SCOPE
  templateUrl: './view-chart-page.component.html',
  styleUrl: './view-chart-page.component.css'
})
export class ViewChartPageComponent {
  protected readonly service = inject(ViewChartService);

  readonly configuringTopic = signal<Memento | null>(null);
  readonly chartData = signal<ChartSeries[]>([]);

  get monthLabel(): string {
    const monthNames = ["January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"
    ];
    return `${monthNames[this.service.selectedMonth() - 1]} ${this.service.selectedYear()}`;
  }

  onConfigureTopic(topicId: number) {
    const topic = this.service.selectedTopics().find(t => t.id === topicId);
    if (topic) {
      this.configuringTopic.set(topic);
    }
  }

  onSaveConfig(setting: ChartSetting) {
    const topic = this.configuringTopic();
    if (topic) {
      this.service.saveTopicSetting(topic.id, setting);
      this.configuringTopic.set(null);
    }
  }

  onCancelConfig() {
    this.configuringTopic.set(null);
  }

  onRender() {
    const series: ChartSeries[] = [];
    const settings = this.service.topicSettings();
    const dataMap = this.service.topicDataMap();
    console.log('Selected Topics:', this.service.selectedTopics());

    this.service.selectedTopics().forEach(topic => {
      const setting = settings.get(topic.id);
      const data = dataMap.get(topic.id);
      console.log('Setting:', setting);
      console.log('Data:', data);

      if (setting && data && data.children) {


        const points: ChartDataPoint[] = data.children.map((child: Memento) => {
          const mapping = setting.labelValues.find(lv => lv.label === child.title);
          return {
            date: child.startDate || '', // Expected format or parse from date
            label: child.title,
            value: mapping ? mapping.value : 0,
            color: mapping ? mapping.color : '#cbd5e0',
            mementoId: child.id
          };
        });

        series.push({
          topicId: topic.id,
          topicTitle: topic.title,
          setting,
          points
        });
      }
    });

    // Sort by order
    series.sort((a, b) => (a.setting.order || 0) - (b.setting.order || 0));
    this.chartData.set(series);
  }
}
