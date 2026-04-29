export interface StreamBook {
  title: string;
  content: string;
}

export interface StreamPost {
  title: string;
  excerpt: string;
  content: string;
  author: string;
  date: string;
}

export interface StreamEntry {
  date: Date;
  label: string; // e.g., "T5", "CN", "T4"
  color: string; // The color of the vertical bar
  dots: string[]; // Array of colors for the circles
  type: 'mood' | 'idea' | 'all';
  books?: StreamBook[];
  posts?: StreamPost[];
}

export interface MonthSummary {
  month: string; // "Jan", "Feb", etc.
  count: number; // e.g., 10, 7
}

export interface YearlyStreamState {
  year: number;
  entries: StreamEntry[];
  filter: 'all' | 'mood' | 'idea';
}
