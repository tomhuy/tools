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

export interface WeeklyEntry {
  id: string;
  date: Date; // Specific hour
  moodId: string;
  tags: string[];
  note?: string;
  reason?: string;
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
