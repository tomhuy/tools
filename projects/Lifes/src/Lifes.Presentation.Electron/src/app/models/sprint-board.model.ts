export interface SprintTask {
  id: string;
  label: string;
  name: string;
  person: string; // 'pre' | 'huy' | 'tuan' | 'bang' | 'hoa'
  done: boolean;
}

export interface SprintFeature {
  id: string;
  name: string;
  accent: string; // 'blue' | 'green' | 'orange' | 'purple'
  tasks: SprintTask[];
}

export interface SprintBoardData {
  features: SprintFeature[];
}

export interface Person {
  id: string;
  label: string;
  initials: string;
  color: string;
}
