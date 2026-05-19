export type Quadrant = 'tl' | 'tr' | 'bl' | 'br';
export type YearKey = 1 | 3 | 5 | 10;

export interface ActivityMetrics {
  compound: [number, number, number, number];
  clarity:  [number, number, number, number];
  network:  [number, number, number, number];
}

export interface ActivityBar {
  label: string;
  color: string;
}

export interface ActivityItem {
  id:         string;
  name:       string;
  icon:       string;
  color:      string;
  colorLight: string;
  timeInvest: number;
  desc:       string;
  quad:       Quadrant;
  pos:        { x: number; y: number };
  metrics:    ActivityMetrics;
  bars:       ActivityBar[];
  insights:   Record<YearKey, string>;
}
