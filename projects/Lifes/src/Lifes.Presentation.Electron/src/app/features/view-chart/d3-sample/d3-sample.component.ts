import { Component, ElementRef, Input, OnChanges, OnInit, ViewChild, SimpleChanges, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartSeries, ChartDataPoint } from '../../../models/chart.model';
import * as d3 from 'd3';

@Component({
  selector: 'app-d3-sample',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="d3-container">
      <div class="header">
        <h4>D3.js Visualization Engine (Premium Design)</h4>
        <p class="sub-text">Dynamic Data-Driven SVG with Professional Aesthetics</p>
      </div>
      <div class="chart-wrapper" #chartContainer></div>
    </div>
  `,
  styles: [`
    .d3-container {
      width: 100%;
      background: white;
      border-radius: 20px;
      padding: 24px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.05);
      border: 1px solid #f1f5f9;
    }
    .header { margin-bottom: 24px; }
    .header h4 { margin: 0; color: #2c3e50; font-size: 16px; font-weight: 700; font-family: 'Segoe UI', sans-serif; }
    .sub-text { margin: 4px 0 0; font-size: 12px; color: #95a5a6; }
    .chart-wrapper { width: 100%; height: 480px; display: block; overflow: visible; font-family: 'Segoe UI', Arial, sans-serif; }
  `]
})
export class D3SampleComponent implements OnInit, OnChanges, AfterViewInit, OnDestroy {
  @Input() chartData: ChartSeries[] = [];
  
  @ViewChild('chartContainer', { static: true }) 
  private chartContainer!: ElementRef;

  private svg: any;
  private width = 0;
  private height = 440;
  private margin = { top: 20, right: 50, bottom: 60, left: 70 };
  private resizeObserver?: ResizeObserver;

  private sections = {
    events: { start: 40, end: 70 },
    emotions: { start: 80, end: 230 },
    sleep: { start: 260, end: 360 }
  };

  ngOnInit() {}

  ngAfterViewInit() {
    setTimeout(() => {
      this.initChart();
      this.setupResizeObserver();
    }, 150);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['chartData'] && this.svg) {
      this.updateChart();
    }
  }

  ngOnDestroy() {
    if (this.resizeObserver) this.resizeObserver.disconnect();
  }

  private setupResizeObserver() {
    this.resizeObserver = new ResizeObserver(() => this.initChart());
    this.resizeObserver.observe(this.chartContainer.nativeElement);
  }

  private initChart() {
    const element = this.chartContainer.nativeElement;
    const currentWidth = element.offsetWidth;
    if (currentWidth <= 0) return;

    this.width = currentWidth - this.margin.left - this.margin.right;
    d3.select(element).selectAll('svg').remove();

    this.svg = d3.select(element)
      .append('svg')
      .attr('width', '100%')
      .attr('height', this.height + this.margin.top + this.margin.bottom)
      .attr('viewBox', `0 0 ${this.width + this.margin.left + this.margin.right} ${this.height + this.margin.top + this.margin.bottom}`)
      .append('g')
      .attr('transform', `translate(${this.margin.left},${this.margin.top})`);

    this.updateChart();
  }

  private updateChart() {
    if (!this.chartData || this.chartData.length === 0 || !this.svg) return;
    this.svg.selectAll('*').remove();

    // --- 1. Scales ---
    const x = d3.scaleLinear().domain([1, 31]).range([0, this.width]);
    const yEmotion = d3.scaleLinear().domain([0, 7]).range([this.sections.emotions.end, this.sections.emotions.start]);
    const ySleep = d3.scaleLinear().domain([4, 9]).range([this.sections.sleep.end, this.sections.sleep.start]);

    // --- 2. Sections ---
    
    // EMOTION SECTION
    const emotionG = this.svg.append('g');
    emotionG.append('text').attr('x', -40).attr('y', 86).text('CẢM XÚC').style('font-weight', 'bold').style('font-size', '10px').style('fill', '#7f8c8d').style('letter-spacing', '1px');
    
    const zones = [
      { y1: 4, y2: 7, color: '#f0faf4' }, { y1: 2, y2: 4, color: '#fefdf0' }, { y1: 0, y2: 2, color: '#fdf5f5' }
    ];
    emotionG.selectAll('.zone').data(zones).enter().append('rect')
      .attr('x', 0).attr('y', (d: any) => yEmotion(d.y2)).attr('width', this.width).attr('height', (d: any) => yEmotion(d.y1) - yEmotion(d.y2))
      .attr('fill', (d: any) => d.color).attr('rx', 2);

    // Emotion Grid Lines
    emotionG.selectAll('.grid-line').data([0, 1.5, 3, 4.5, 6, 7.5]).enter().append('line')
      .attr('x1', 0).attr('x2', this.width).attr('y1', (d: any) => yEmotion(d)).attr('y2', (d: any) => yEmotion(d))
      .attr('stroke', '#ebebeb').attr('stroke-width', 0.5);

    const emotionLabels = ['D', 'C-', 'C', 'C+', 'B-', 'B', 'B+', 'A'];
    emotionG.selectAll('.y-label').data(d3.range(8)).enter().append('text')
      .attr('x', -10).attr('y', (d: any) => yEmotion(d) + 4).attr('text-anchor', 'end').text((d: any) => emotionLabels[d])
      .style('font-size', '9px').style('font-weight', 'bold').style('fill', (d: any) => d === 0 ? '#e74c3c' : '#95a5a6');

    // SLEEP SECTION
    const sleepG = this.svg.append('g');
    sleepG.append('text').attr('x', -40).attr('y', 268).text('NGỦ').style('font-weight', 'bold').style('font-size', '10px').style('fill', '#7f8c8d').style('letter-spacing', '1px');

    sleepG.selectAll('.y-label').data([4, 5.5, 7, 8.5]).enter().append('text')
      .attr('x', -10).attr('y', (d: any) => ySleep(d) + 4).attr('text-anchor', 'end').text((d: any) => d + 'h')
      .style('font-size', '9px').style('fill', '#95a5a6');

    // 7h Target Line
    sleepG.append('line').attr('x1', 0).attr('x2', this.width).attr('y1', ySleep(7)).attr('y2', ySleep(7))
      .attr('stroke', '#2ecc71').attr('stroke-width', 0.8).attr('stroke-dasharray', '6,4').attr('opacity', 0.3);
    sleepG.append('text').attr('x', this.width + 5).attr('y', ySleep(7) + 3).text('7h ★').style('font-size', '8px').style('fill', '#27ae60');

    // EVENT SECTION
    const eventG = this.svg.append('g');

    // --- 3. Data Rendering ---
    this.chartData.forEach((series, idx) => {
      const isSleep = series.topicTitle.toLowerCase().includes('ngủ') || series.setting.chartType === 'bar';
      const isEvent = series.topicTitle.toLowerCase().includes('học') || series.topicTitle.toLowerCase().includes('làm');

      if (isSleep) {
        sleepG.selectAll(`.bar-${series.topicId}`).data(series.points).enter().append('rect')
          .attr('x', (d: any) => x(this.getDayFromDate(d.date)) - 9).attr('y', (d: any) => ySleep(d.value))
          .attr('width', 18).attr('height', (d: any) => this.sections.sleep.end - ySleep(d.value))
          .attr('fill', (d: any) => d.color).attr('rx', 2);
      } else if (isEvent) {
        const eventY = this.sections.events.start + (idx * 14);
        eventG.append('text').attr('x', -10).attr('y', eventY + 4).attr('text-anchor', 'end').text(series.topicTitle.substring(0, 3).toUpperCase())
          .style('font-size', '9px').style('font-weight', 'bold').attr('fill', this.getSeriesColor(series));

        eventG.selectAll(`.event-${series.topicId}`).data(series.points).enter().append('circle')
          .attr('cx', (d: any) => x(this.getDayFromDate(d.date))).attr('cy', eventY).attr('r', 4).attr('fill', this.getSeriesColor(series));
        
        // Vertical Event Guide Lines
        eventG.selectAll(`.guide-${series.topicId}`).data(series.points).enter().append('line')
          .attr('x1', (d: any) => x(this.getDayFromDate(d.date))).attr('x2', (d: any) => x(this.getDayFromDate(d.date)))
          .attr('y1', 52).attr('y2', 242).attr('stroke', this.getSeriesColor(series)).attr('stroke-width', 1).attr('opacity', 0.1);
      } else {
        const line = d3.line<ChartDataPoint>().x(d => x(this.getDayFromDate(d.date))).y(d => yEmotion(d.value)).curve(d3.curveMonotoneX);
        emotionG.append('path').datum(series.points).attr('fill', 'none').attr('stroke', '#bdc3c7').attr('stroke-width', 1.5).attr('stroke-dasharray', '4,3').attr('d', line);
        emotionG.selectAll(`.dot-${series.topicId}`).data(series.points).enter().append('circle')
          .attr('cx', (d: any) => x(this.getDayFromDate(d.date))).attr('cy', (d: any) => yEmotion(d.value))
          .attr('r', 6).attr('fill', (d: any) => d.color).attr('stroke', 'white').attr('stroke-width', 2);
      }
    });

    // --- 4. Today Marker & Axis ---
    const today = new Date().getDate();
    this.svg.append('line').attr('x1', x(today)).attr('x2', x(today)).attr('y1', 52).attr('y2', 377)
      .attr('stroke', '#f39c12').attr('stroke-width', 0.8).attr('stroke-dasharray', '3,4').attr('opacity', 0.4);
    this.svg.append('text').attr('x', x(today)).attr('y', 395).attr('text-anchor', 'middle').text('hôm nay')
      .style('font-size', '8px').style('fill', '#f39c12').attr('opacity', 0.8);

    const xAxis = d3.axisBottom(x).tickValues([1, 5, 10, 15, 20, 25, 31]).tickSizeOuter(0);
    this.svg.append('g').attr('transform', `translate(0, 377)`).call(xAxis).attr('color', '#ddd')
      .selectAll('text').style('font-size', '10px').style('fill', '#bdc3c7');

    // --- 5. Legend ---
    const legendG = this.svg.append('g').attr('transform', `translate(0, 420)`);
    let legendX = 0;
    this.chartData.forEach((series) => {
      const g = legendG.append('g').attr('transform', `translate(${legendX}, 0)`);
      g.append('circle').attr('r', 4).attr('fill', this.getSeriesColor(series));
      g.append('text').attr('x', 10).attr('y', 4).text(series.topicTitle).style('font-size', '10px').style('fill', '#7f8c8d');
      legendX += series.topicTitle.length * 7 + 30;
    });
  }

  private getSeriesColor(series: ChartSeries): string {
    return series.setting.labelValues[0]?.color || '#0ea5e9';
  }

  private getDayFromDate(dateStr: string): number {
    try {
      const date = new Date(dateStr);
      return date.getDate();
    } catch { return 1; }
  }
}
