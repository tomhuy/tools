export enum EnergyGrade {
  A = 'A',
  B_PLUS = 'B+',
  B = 'B',
  B_MINUS = 'B-',
  C_PLUS = 'C+',
  C = 'C',
  C_MINUS = 'C-',
  D = 'D'
}

export interface EnergyLevelInfo {
  grade: EnergyGrade;
  label: string;
  color: string;
}

export interface ActivityTag {
  id: string;
  label: string;
}

export interface DailyEntry {
  hour: number; // 0 to 23
  energyGrade?: EnergyGrade;
  tags: string[]; // tag labels
  note?: string;
}

export const ENERGY_LEVELS: EnergyLevelInfo[] = [
  { grade: EnergyGrade.A, label: 'High energy, flow', color: '#7fc9a4' },
  { grade: EnergyGrade.B_PLUS, label: 'Đầu óc thả lỏng', color: '#88b69e' },
  { grade: EnergyGrade.B, label: 'Bình thường, ổn định', color: '#a8c365' },
  { grade: EnergyGrade.B_MINUS, label: 'Việc bớt, chưa mãn nguyện', color: '#d1a04d' },
  { grade: EnergyGrade.C_PLUS, label: 'Mệt, hiệu suất giảm', color: '#c57f5a' },
  { grade: EnergyGrade.C, label: 'Căng thẳng', color: '#b9605c' },
  { grade: EnergyGrade.C_MINUS, label: 'Kiệt sức', color: '#9d4a4d' },
  { grade: EnergyGrade.D, label: 'Burnout', color: '#7c383a' }
];
