export interface LabelValue {
  label: string;
  value: number;
  color: string;
}

export type ChartType = 'line' | 'bar' | 'scatter';

export interface ChartSetting {
  chartType: ChartType;
  labelValues: LabelValue[];
  order: number;
}

export interface ChartDataPoint {
  date: string;
  label: string;
  value: number;
  color: string;
  mementoId: number;
}

export interface ChartSeries {
  topicId: number;
  topicTitle: string;
  setting: ChartSetting;
  points: ChartDataPoint[];
}
