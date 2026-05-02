export interface Note {
  id: string;
  title: string;
  content: string;
  tags: string[];
  section: string | null;
  starred: boolean;
  modified: Date | string;
}

export interface NavItem {
  id: string;
  label: string;
  icon: string;
  badge?: number;
  color?: string;
}

export interface NavSection {
  id: string;
  label: string;
  items: NavItem[];
}

export interface NoteQuery {
  queryType: 'inbox' | 'all' | 'starred' | 'section';
  section?: string;
  page: number;
  pageSize: number;
  search?: string;
  sort?: string;
}
