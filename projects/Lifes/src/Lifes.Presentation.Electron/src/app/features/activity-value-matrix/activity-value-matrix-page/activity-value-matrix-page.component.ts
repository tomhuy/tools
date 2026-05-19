import { Component, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivityValueMatrixService, YEAR_INDEX } from '../activity-value-matrix.service';
import { BubbleAreaComponent } from '../bubble-area/bubble-area.component';
import { ActivityItem, YearKey } from '../models/activity-value-matrix.model';

const QUADRANTS = [
  { key: 'tl', label: 'Cao giá trị · Ít thời gian',    labelColor: '#0F6E56', bg: 'rgba(29,158,117,0.06)'   },
  { key: 'tr', label: 'Cao giá trị · Nhiều thời gian',  labelColor: '#185FA5', bg: 'rgba(55,138,221,0.06)'   },
  { key: 'bl', label: 'Thấp giá trị · Ít thời gian',   labelColor: '#BA7517', bg: 'rgba(201,176,106,0.08)'  },
  { key: 'br', label: 'Thấp giá trị · Nhiều thời gian', labelColor: '#993556', bg: 'rgba(212,83,126,0.06)'   },
] as const;

const YEARS: YearKey[] = [1, 3, 5, 10];

@Component({
  selector: 'app-activity-value-matrix-page',
  standalone: true,
  imports: [CommonModule, BubbleAreaComponent],
  templateUrl: './activity-value-matrix-page.component.html',
  styleUrl: './activity-value-matrix-page.component.css',
})
export class ActivityValueMatrixPageComponent implements OnInit {
  private svc = inject(ActivityValueMatrixService);

  ngOnInit(): void {
    this.svc.selectActivity('note');
  }

  readonly currentYear = this.svc.currentYear;
  readonly selectedId = this.svc.selectedId;
  readonly selectedActivity = this.svc.selectedActivity;

  readonly quadrants = QUADRANTS;
  readonly years = YEARS;

  readonly activitiesByQuad = computed(() => {
    const all = this.svc.activities();
    return Object.fromEntries(
      QUADRANTS.map(q => [q.key, all.filter(a => a.quad === q.key)])
    ) as Record<string, ActivityItem[]>;
  });

  readonly detailMetrics = computed(() => {
    const a = this.selectedActivity();
    if (!a) return null;
    const idx = YEAR_INDEX[this.currentYear()];
    const compound = a.metrics.compound[idx];
    const clarity  = a.metrics.clarity[idx];
    const network  = a.metrics.network[idx];
    return {
      compound,
      clarity,
      network,
      barValues: [clarity, network, Math.round(compound * 10)],
      insight: a.insights[this.currentYear()],
    };
  });

  setYear(year: YearKey): void {
    this.svc.setYear(year);
  }

  onBubbleClick(id: string): void {
    this.svc.selectActivity(id);
  }

  trackByKey(_: number, q: typeof QUADRANTS[number]): string {
    return q.key;
  }
}
