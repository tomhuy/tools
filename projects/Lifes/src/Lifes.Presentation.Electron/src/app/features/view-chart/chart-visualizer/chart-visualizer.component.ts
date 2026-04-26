import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartSeries, ChartDataPoint } from '../../../models/chart.model';

@Component({
  selector: 'app-chart-visualizer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="visualizer-container">
      <svg [attr.viewBox]="viewBox" preserveAspectRatio="xMidYMid meet">
        <defs>
          <marker id="arr" markerWidth="6" markerHeight="6" refX="5" refY="3" orient="auto">
            <path d="M0,0 L6,3 L0,6 Z" fill="#e74c3c" opacity="0.7"/>
          </marker>
        </defs>

        <!-- Background -->
        <rect width="780" height="440" fill="#ffffff" rx="8"/>
        <rect x="1" y="1" width="778" height="438" fill="none" stroke="#ecf0f1" stroke-width="1" rx="8"/>

        <!-- Title -->
        <text x="390" y="25" text-anchor="middle" class="chart-title">Phân Tích Dữ Liệu Tổng Hợp — {{ monthLabel }}</text>
        <text x="390" y="41" text-anchor="middle" class="chart-subtitle">Tác nhân — Phản ứng — Phục hồi</text>

        <!-- ====== SECTION 1: EVENTS (y=52–82) ====== -->
        <g class="section-events">
          @for (series of eventSeries; track series.topicId; let outerIndex = $index) {
            <text x="74" [attr.y]="63 + (outerIndex * 14)" text-anchor="end" class="row-label" [style.fill]="getSeriesColor(series)">
              {{ series.topicTitle.substring(0, 3).toUpperCase() }}
            </text>
            @for (point of series.points; track point.mementoId) {
              <circle [attr.cx]="getXPos(getDayFromDate(point.date))" [attr.cy]="60 + (outerIndex * 14)" r="4" [attr.fill]="getSeriesColor(series)"/>
              <line 
                [attr.x1]="getXPos(getDayFromDate(point.date))" 
                y1="52" 
                [attr.x2]="getXPos(getDayFromDate(point.date))" 
                y2="242" 
                [attr.stroke]="getSeriesColor(series)" 
                stroke-width="1" 
                opacity="0.1"
              />
            }
          }
        </g>

        <!-- ====== SECTION 2: EMOTIONS (y=90–240) ====== -->
        <g class="section-emotions">
          <text x="30" y="96" class="section-tag">CẢM XÚC</text>
          
          <!-- Zones -->
          <rect x="80" y="90"  width="660" height="64" fill="#f0faf4" rx="2"/>
          <rect x="80" y="154" width="660" height="65" fill="#fefdf0" rx="2"/>
          <rect x="80" y="219" width="660" height="21" fill="#fdf5f5" rx="2"/>

          <!-- Y axis labels -->
          <text x="74" y="243" text-anchor="end" class="y-lbl" fill="#e74c3c" font-weight="bold">D</text>
          <text x="74" y="222" text-anchor="end" class="y-lbl" fill="#e67e22">C-</text>
          <text x="74" y="200" text-anchor="end" class="y-lbl" fill="#e67e22">C</text>
          <text x="74" y="179" text-anchor="end" class="y-lbl" fill="#f39c12">C+</text>
          <text x="74" y="157" text-anchor="end" class="y-lbl" fill="#d4ac0d">B-</text>
          <text x="74" y="136" text-anchor="end" class="y-lbl" fill="#27ae60">B</text>
          <text x="74" y="114" text-anchor="end" class="y-lbl" fill="#1e8449">B+</text>

          @for (series of emotionSeries; track series.topicId) {
            <!-- Dashed Path -->
            <path [attr.d]="getEmotionPath(series.points)" fill="none" stroke="#bdc3c7" stroke-width="1.5" stroke-dasharray="4,3"/>
            
            @for (point of series.points; track point.mementoId) {
              <circle 
                [attr.cx]="getXPos(getDayFromDate(point.date))" 
                [attr.cy]="getEmotionYPos(point.value)" 
                r="7" 
                [attr.fill]="point.color"
              />
            }
          }
        </g>

        <!-- ====== SECTION 3: SLEEP (y=272–372) ====== -->
        <g class="section-sleep">
          <text x="30" y="278" class="section-tag">NGỦ</text>
          
          <!-- Y axis labels -->
          <text x="74" y="375" text-anchor="end" class="y-lbl" fill="#95a5a6">4h</text>
          <text x="74" y="315" text-anchor="end" class="y-lbl" fill="#95a5a6">7h</text>
          <text x="74" y="275" text-anchor="end" class="y-lbl" fill="#95a5a6">9h</text>

          <!-- 7h Target Line -->
          <line x1="80" y1="312" x2="740" y2="312" stroke="#2ecc71" stroke-width="0.8" stroke-dasharray="6,4" opacity="0.3"/>
          
          @for (series of sleepSeries; track series.topicId) {
            @for (point of series.points; track point.mementoId) {
              <rect 
                [attr.x]="getXPos(getDayFromDate(point.date)) - 9" 
                [attr.y]="getSleepYPos(point.value)" 
                width="18" 
                [attr.height]="372 - getSleepYPos(point.value)" 
                [attr.fill]="point.color" 
                rx="2"
              />
              @if (point.value >= 8 || point.value <= 4.5) {
                <text 
                  [attr.x]="getXPos(getDayFromDate(point.date))" 
                  [attr.y]="getSleepYPos(point.value) - 4" 
                  text-anchor="middle" 
                  class="ann-text" 
                  [attr.fill]="point.value >= 8 ? '#16a085' : '#c0392b'"
                >{{ point.value }}h</text>
              }
            }
          }
        </g>

        <!-- ====== X AXIS ====== -->
        <line x1="80" y1="377" x2="740" y2="377" stroke="#ddd" stroke-width="1"/>
        @for (day of [1, 5, 10, 15, 20, 25, 31]; track day) {
          <text 
            [attr.x]="getXPos(day)" 
            y="389" 
            text-anchor="middle" 
            class="y-lbl" 
            [class.important]="day === 10 || day === 15"
          >{{ day }}</text>
        }
      </svg>

      <!-- LEGEND -->
      <div class="custom-legend">
        @for (series of chartData; track series.topicId) {
          <div class="legend-item">
            <span class="legend-dot" [style.background-color]="getSeriesColor(series)"></span>
            <span class="legend-label">{{ series.topicTitle }}</span>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .visualizer-container { width: 100%; height: 100%; padding: 10px; }
    svg { width: 100%; height: auto; font-family: 'Segoe UI', Arial, sans-serif; }
    
    .chart-title { font-weight: bold; font-size: 14px; fill: #2c3e50; }
    .chart-subtitle { font-size: 10px; fill: #95a5a6; }
    .section-tag { font-weight: bold; font-size: 10px; fill: #7f8c8d; letter-spacing: 1px; }
    .row-label { font-weight: bold; font-size: 9px; }
    .y-lbl { font-size: 9px; fill: #95a5a6; }
    .y-lbl.important { fill: #e74c3c; font-weight: bold; }
    .ann-text { font-size: 8px; font-weight: bold; }
    
    .custom-legend {
      display: flex;
      flex-wrap: wrap;
      justify-content: center;
      gap: 16px;
      margin-top: 10px;
    }
    .legend-item { display: flex; align-items: center; gap: 6px; }
    .legend-dot { width: 8px; height: 8px; border-radius: 50%; }
    .legend-label { font-size: 11px; color: #7f8c8d; }
  `]
})
export class ChartVisualizerComponent implements OnChanges {
  @Input() chartData: ChartSeries[] = [];
  @Input() monthLabel: string = 'April 2026';
  
  viewBox = '0 0 780 440';
  
  eventSeries: ChartSeries[] = [];
  emotionSeries: ChartSeries[] = [];
  sleepSeries: ChartSeries[] = [];

  ngOnChanges(changes: SimpleChanges) {
    if (changes['chartData']) {
      this.categorizeSeries();
    }
  }

  categorizeSeries() {
    // Logic to split series into sections based on title or type
    // In a real app, this could be based on a 'category' property
    this.eventSeries = this.chartData.filter(s => s.topicTitle.toLowerCase().includes('học') || s.topicTitle.toLowerCase().includes('làm'));
    this.emotionSeries = this.chartData.filter(s => s.setting.chartType === 'line' || s.topicTitle.toLowerCase().includes('cảm xúc'));
    this.sleepSeries = this.chartData.filter(s => s.setting.chartType === 'bar' || s.topicTitle.toLowerCase().includes('ngủ'));
  }

  getXPos(day: number): number {
    const startX = 93; // Matches sample for day 1
    const endX = 727;   // Matches sample for day 25/today
    // Based on sample: x increases by ~26-27 per day
    const dayGap = (727 - 93) / 24; 
    return startX + (day - 1) * dayGap;
  }

  getEmotionYPos(value: number): number {
    // value 0(D) -> 240, 6(B+) -> 114
    const dPos = 240;
    const bPlusPos = 114;
    const step = (dPos - bPlusPos) / 6;
    return dPos - (value * step);
  }

  getSleepYPos(value: number): number {
    // 4h -> 372, 7h -> 312, 9h -> 272
    // 1h = 20px
    const base4h = 372;
    return base4h - (value - 4) * 20;
  }

  getEmotionPath(points: ChartDataPoint[]): string {
    if (points.length === 0) return '';
    const sorted = [...points].sort((a, b) => this.getDayFromDate(a.date) - this.getDayFromDate(b.date));
    return sorted.reduce((path, p, i) => {
      const x = this.getXPos(this.getDayFromDate(p.date));
      const y = this.getEmotionYPos(p.value);
      return path + (i === 0 ? `M ${x} ${y}` : ` L ${x} ${y}`);
    }, '');
  }

  getSeriesColor(series: ChartSeries): string {
    return series.setting.labelValues[0]?.color || '#0ea5e9';
  }

  getDayFromDate(dateStr: string): number {
    try {
      const date = new Date(dateStr);
      return date.getDate();
    } catch { return 1; }
  }
}
