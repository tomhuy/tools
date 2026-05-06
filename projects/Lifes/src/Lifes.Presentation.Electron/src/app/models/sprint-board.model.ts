export interface SprintTask {
  id: string;
  label: string;
  name: string;
  assigneeId: string; // 'pre' | userId
  done: boolean;
  isTopPriority?: boolean;
}

export interface SprintFeature { // Epic in Backend
  id: string;
  name: string;
  color: string;
  archived: boolean;
  status: 'progress' | 'backlog';
  tasks: SprintTask[];
}

export interface SprintBoardData {
  features: SprintFeature[];
}
