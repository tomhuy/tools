import { Injectable, signal, computed } from '@angular/core';

export interface Note {
  id: number;
  title: string;
  content: string;
  tags: string[];
  section: string | null;
  starred: boolean;
  modified: Date;
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

@Injectable({
  providedIn: 'root'
})
export class LaputaNotesService {

  // Colors mapping for sections
  public readonly COLORS: { [key: string]: string } = {
    amber: '#C9B06A', purple: '#7F77DD', teal: '#1D9E75',
    red: '#D4537E', blue: '#378ADD'
  };

  public readonly ALL_TAGS = ['tâm lý học', 'design', 'journal', 'ideas', 'GTD', 'habit', 'đọc sách', 'coding'];

  // State Signals
  public viewMode = signal<'list' | 'card' | 'compact' | 'grid'>('card');
  public theme = signal<'dark' | 'sepia'>('dark');
  public currentSection = signal<string>('all');
  public currentNoteId = signal<number | null>(null);
  public isPreview = signal<boolean>(false);
  public isSidebarOpen = signal<boolean>(true);
  public searchQuery = signal<string>('');
  public sortAsc = signal<boolean>(false);

  // Data Signals
  public navConfig = signal<{ pinned: NavItem[], sections: NavSection[] }>({
    pinned: [
      { id: 'inbox', label: 'Inbox', icon: 'inbox', badge: 3 },
      { id: 'all', label: 'Tất cả', icon: 'all' },
      { id: 'starred', label: 'Đánh dấu sao', icon: 'star' },
    ],
    sections: [
      {
        id: 'types', label: 'Types', items: [
          { id: 'journal', label: 'Journal', icon: '📓', color: 'amber' },
          { id: 'ideas', label: 'Ý tưởng', icon: '💡', color: 'purple' },
          { id: 'resources', label: 'Tài nguyên', icon: '📚', color: 'teal' },
          { id: 'tasks', label: 'Nhiệm vụ', icon: '✅', color: 'blue' },
          { id: 'psychology', label: 'Tâm lý học', icon: '🧠', color: 'red' },
        ]
      },
      {
        id: 'folders', label: 'Folders', items: [
          { id: 'personal', label: 'Cá nhân', icon: '📁', color: 'amber' },
          { id: 'work', label: 'Công việc', icon: '📁', color: 'blue' },
          { id: 'learning', label: 'Học tập', icon: '📁', color: 'purple' },
        ]
      }
    ]
  });

  public notes = signal<Note[]>([
    { id: 1, title: 'Zeigarnik Effect & 15 phút cuối ngày', content: `# Zeigarnik Effect & 15 phút cuối ngày\n\nNão bộ **không cần bạn hoàn thành 100% việc** — nó chỉ cần một tín hiệu đủ thuyết phục rằng việc đó "đã được xử lý".\n\n## Cơ chế hoạt động\n\nKhi một nhiệm vụ còn dở dang, não bộ giữ một "loop" mở trong working memory. Loop này:\n\n- Tiêu thụ cognitive resources liên tục\n- Gây cảm giác bồn chồn, khó tập trung\n- Tự động kích hoạt lại khi có liên kết bất kỳ\n\n## Giải pháp\n\n15 phút đọc trước khi ngủ không phải "hoàn thành việc" — mà là *tín hiệu đủ để não bộ coi việc đó là "đã xử lý"* và cho phép buông.\n\n> "Capturing" trong GTD của David Allen hoạt động theo nguyên lý này.\n\n## Ứng dụng cá nhân\n\n1. Review note ngắn cuối ngày\n2. Viết 1–2 câu về việc còn dở\n3. Não sẽ cho phép ngủ ngon hơn`, tags: ['tâm lý học', 'GTD'], section: 'psychology', starred: true, modified: new Date(Date.now() - 3600000 * 2) },
    { id: 2, title: 'Self-compassion — Kristin Neff', content: `# Self-Compassion theo Kristin Neff\n\n## Ba thành phần cốt lõi\n\n1. **Self-kindness** — đối xử với bản thân như với người bạn\n2. **Common humanity** — nhận ra rằng đau khổ là phần của trải nghiệm con người\n3. **Mindfulness** — quan sát cảm xúc không phán xét\n\n## Phân biệt với self-esteem\n\nSelf-esteem thường phụ thuộc vào kết quả bên ngoài. Self-compassion thì không — nó ổn định hơn và bền vững hơn.\n\n*Đọc: Self-Compassion — The Proven Power of Being Kind to Yourself*`, tags: ['tâm lý học', 'đọc sách'], section: 'psychology', starred: false, modified: new Date(Date.now() - 86400000) },
    { id: 3, title: 'Hệ thống màu sắc cho emotion tracker', content: `# Hệ thống màu sắc — Emotion Tracker\n\n## Nguyên tắc\n\nMàu sắc nên encode **ý nghĩa tâm lý**, không phải danh mục.\n\n## Ramp cảm xúc\n\n| Level | Màu | Hex |\n|-------|-----|-----|\n| A — Flow | Teal | #1D9E75 |\n| B+ — Thả lỏng | Teal nhạt | #5DCAA5 |\n| B — Ổn định | Xanh lá | #97C459 |\n| B- — Chưa mãn | Amber | #EF9F27 |\n| C+ — Mệt | Coral | #D85A30 |\n\n## Lý do dùng ramp đơn\n\nNão bộ xử lý gradient màu tự nhiên theo thang nhiệt độ. Một ramp liên tục giúp đọc pattern ngay lập tức mà không cần đọc legend.`, tags: ['design', 'color'], section: 'ideas', starred: true, modified: new Date(Date.now() - 86400000 * 2) },
    { id: 4, title: 'Flow — Csikszentmihalyi', content: `# Flow: The Psychology of Optimal Experience\n\n## Điều kiện để đạt Flow\n\n- **Challenge–skill balance**: thách thức vừa đủ, không quá dễ cũng không quá khó\n- Clear goals, immediate feedback\n- Deep concentration — không bị ngắt\n\n## Mô hình 8 trạng thái\n\nTừ Boredom đến Anxiety, Flow nằm ở điểm cân bằng giữa kỹ năng cao và thách thức cao.\n\n## Ứng dụng cá nhân\n\n- Coding với AI: thường đạt flow khi bài toán vừa sức\n- Đọc sách: cần môi trường yên tĩnh`, tags: ['đọc sách', 'psychology'], section: 'resources', starred: false, modified: new Date(Date.now() - 86400000 * 3) },
    { id: 5, title: 'Morning routine — thử nghiệm', content: `# Morning Routine — Thử nghiệm Q2 2026\n\n## Cấu trúc hiện tại\n\n- 6:00 — Thức dậy, uống nước\n- 6:15 — 15 phút đọc sách (không điện thoại)\n- 6:30 — Ghi chú ngắn: 3 điều cần làm hôm nay\n- 7:00 — Bắt đầu deep work block đầu tiên\n\n## Nhận xét sau 2 tuần\n\nNhững buổi sáng theo routine này thường có **mood B+** trong suốt buổi sáng. Cảm giác kiểm soát cao hơn.\n\n> Không check điện thoại trước 8h là yếu tố quan trọng nhất.`, tags: ['habit', 'journal'], section: 'journal', starred: false, modified: new Date(Date.now() - 86400000 * 5) },
  ]);

  private nextId = 6;

  // Computed properties
  public filteredNotes = computed(() => {
    let filtered = this.notes().filter(n => {
      const sec = this.currentSection();
      if (sec === 'starred') return n.starred;
      if (sec === 'inbox') return !n.section;
      if (sec !== 'all') return n.section === sec;
      return true;
    });

    const query = this.searchQuery().toLowerCase();
    if (query) {
      filtered = filtered.filter(n => n.title.toLowerCase().includes(query) || n.content.toLowerCase().includes(query));
    }

    filtered.sort((a, b) => this.sortAsc() ? a.modified.getTime() - b.modified.getTime() : b.modified.getTime() - a.modified.getTime());
    return filtered;
  });

  public currentNote = computed(() => {
    const id = this.currentNoteId();
    if (!id) return null;
    return this.notes().find(n => n.id === id) || null;
  });

  // Actions
  public addNote(title: string, tags: string[]) {
    const section = (this.currentSection() === 'all' || this.currentSection() === 'inbox') ? null : this.currentSection();
    const newNote: Note = {
      id: this.nextId++,
      title: title || 'Ghi chú mới',
      content: `# ${title || 'Ghi chú mới'}\n\n`,
      tags: [...tags],
      section,
      starred: false,
      modified: new Date()
    };
    this.notes.update(n => [newNote, ...n]);
    this.currentNoteId.set(newNote.id);
  }

  public updateNote(id: number, updates: Partial<Note>) {
    this.notes.update(notes => notes.map(n => n.id === id ? { ...n, ...updates, modified: new Date() } : n));
  }

  public deleteNote(id: number) {
    this.notes.update(notes => notes.filter(n => n.id !== id));
    if (this.currentNoteId() === id) {
      this.currentNoteId.set(null);
    }
  }

  public duplicateNote(id: number) {
    const note = this.notes().find(n => n.id === id);
    if (!note) return;
    const dup: Note = {
      ...note,
      id: this.nextId++,
      title: note.title + ' (copy)',
      modified: new Date()
    };
    this.notes.update(n => [dup, ...n]);
  }

  public toggleStar(id: number) {
    this.notes.update(notes => notes.map(n => n.id === id ? { ...n, starred: !n.starred } : n));
  }

  public getCountForSection(id: string): number {
    const allNotes = this.notes();
    if (id === 'all') return allNotes.length;
    if (id === 'starred') return allNotes.filter(n => n.starred).length;
    if (id === 'inbox') return allNotes.filter(n => !n.section).length;
    return allNotes.filter(n => n.section === id).length;
  }
}
