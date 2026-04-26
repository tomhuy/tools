export interface Memento {
  id: number;
  title: string;
  parentId: number | null;
  startDate: string; // ISO string
  endDate: string;   // ISO string
  order: number;
  color?: string;
  tagIds: number[];
  isAchieved: boolean;
  children?: Memento[];
}
