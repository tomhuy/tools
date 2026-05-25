export interface MoodConfig {
  id: string;
  label: string;
  color: string;
  bgSecondary: string;
  weight: number;
}

export interface ActivityTag {
  id: string;
  label: string;
  icon?: string;
}

export interface MoodEntry {
  id: string;
  date: Date; // Specific hour
  moodId: string;
  tags: string[];
  note?: string;
  reason?: string;
  metadata?: { [key: string]: any };
}

export interface MoodMetadataDefinition {
  key: string; // unique, e.g. luong_nuoc_uong
  labelDisplay: string;
  description?: string;
  inputType: string; // text, number, select, checkbox, textarea, etc.
  options?: string[]; // for select, radio, checkbox options list
  enabled: boolean;
}

export interface DaySummary {
  date: Date;
  dayLabel: string; // T2, T3...
  dayNumber: number; // 27, 28...
  dots: string[]; // Colors for mood dots
  isToday: boolean;
}

export type DisplayMode = 'both' | 'mood' | 'action' | 'reason';
export type FilterMode = 'all' | 'equal' | 'above' | 'below';
export type ViewMode = 'cards' | 'intensity';

export interface PatternAidSettings {
  hourlyAvgRibbon: boolean;
  dayMiniSummary: boolean;
  alignmentGuidesOnHover: boolean;
  highlightRecurringSlump: boolean;
}

export interface ColorPalette {
  id: string;
  label: string;
  // Both arrays: index 0 = weight 1 (D/worst) → index 7 = weight 8 (A/best)
  fg: string[]; // Saturated: left border + mood letter
  bg: string[]; // Translucent: cell fill background
}

export const PALETTES: ColorPalette[] = [
  {
    id: 'default',
    label: 'Default',
    fg: ['#991B1B', '#EF4444', '#F87171', '#FB923C', '#FDBA74', '#A9F1A4', '#74EAB4', '#1D9E75'],
    bg: [
      'rgba(153,27,27,0.60)', 'rgba(239,68,68,0.55)', 'rgba(248,113,113,0.50)',
      'rgba(251,146,60,0.50)', 'rgba(253,186,116,0.45)', 'rgba(169,241,164,0.38)',
      'rgba(116,234,180,0.42)', 'rgba(29,158,117,0.48)',
    ],
  },
  {
    id: 'sky-ghibli',
    label: 'Sky — Ghibli landscape',
    fg: ['#6a1810', '#a83020', '#d94a2a', '#e88a3a', '#e8c63a', '#f7f3e8', '#7fb4dc', '#aacde8'],
    bg: [
      'oklch(0.35 0.16 22 / 0.95)', 'oklch(0.45 0.17 25 / 0.90)',
      'oklch(0.52 0.17 35 / 0.85)', 'oklch(0.55 0.15 60 / 0.75)',
      'oklch(0.55 0.13 90 / 0.65)', 'oklch(0.55 0.02 85 / 0.35)',
      'oklch(0.50 0.12 240 / 0.55)', 'oklch(0.55 0.10 230 / 0.45)',
    ],
  },
];

export const MOODS: MoodConfig[] = [
  { id: 'A', label: 'A', color: '#1D9E75', bgSecondary: 'rgba(29, 158, 117, 0.15)', weight: 8 },
  { id: 'B+', label: 'B+', color: '#74EAB4', bgSecondary: 'rgba(116, 234, 180, 0.15)', weight: 7 },
  { id: 'B', label: 'B', color: '#A9F1A4', bgSecondary: 'rgba(169, 241, 164, 0.15)', weight: 6 },
  { id: 'B-', label: 'B-', color: '#FDBA74', bgSecondary: 'rgba(253, 186, 116, 0.15)', weight: 5 },
  { id: 'C+', label: 'C+', color: '#FB923C', bgSecondary: 'rgba(251, 146, 60, 0.15)', weight: 4 },
  { id: 'C', label: 'C', color: '#F87171', bgSecondary: 'rgba(248, 113, 113, 0.15)', weight: 3 },
  { id: 'C-', label: 'C-', color: '#EF4444', bgSecondary: 'rgba(239, 68, 68, 0.15)', weight: 2 },
  { id: 'D', label: 'D', color: '#991B1B', bgSecondary: 'rgba(153, 27, 27, 0.15)', weight: 1 },
];

export const ACTIVITY_TAGS: ActivityTag[] = [
  { id: 'work_alone', label: 'Làm việc một mình' },
  { id: 'meeting', label: 'Họp' },
  { id: 'friends', label: 'Gặp bạn bè' },
  { id: 'family', label: 'Ở cùng gia đình' },
  { id: 'exercise', label: 'Tập thể dục' },
  { id: 'reading', label: 'Đọc sách' },
  { id: 'music', label: 'Nghe nhạc' },
  { id: 'eating', label: 'Ăn uống' },
  { id: 'moving', label: 'Di chuyển' },
  { id: 'resting', label: 'Nghỉ ngơi' },
  { id: 'coding', label: 'Coding' },
  { id: 'movie', label: 'Xem phim' },
];
