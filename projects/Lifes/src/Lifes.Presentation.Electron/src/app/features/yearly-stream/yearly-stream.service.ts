import { Injectable, signal, computed } from '@angular/core';
import { StreamEntry, YearlyStreamState } from '../../models/yearly-stream.model';

@Injectable({
  providedIn: 'root'
})
export class YearlyStreamService {
  private state = signal<YearlyStreamState>({
    year: 2026,
    entries: this.generateMockEntries(),
    filter: 'all'
  });

  year = computed(() => this.state().year);
  filter = computed(() => this.state().filter);
  
  allEntries = computed(() => this.state().entries);
  
  filteredEntries = computed(() => {
    const s = this.state();
    if (s.filter === 'all') return s.entries;
    return s.entries.filter(e => e.type === s.filter || e.type === 'all');
  });

  setFilter(filter: 'all' | 'mood' | 'idea') {
    this.state.update(s => ({ ...s, filter }));
  }

  setYear(year: number) {
    this.state.update(s => ({ ...s, year }));
  }

  private generateMockEntries(): StreamEntry[] {
    const entries: StreamEntry[] = [
      // January
      { 
        date: new Date(2026, 0, 1), 
        label: 'T5', 
        color: '#4caf50', 
        dots: ['#4caf50'], 
        type: 'all',
        books: [
          { title: 'The Daily Stoic', content: 'Today is a good day to practice virtue. Focus on what you can control and let go of the rest.' },
          { title: 'Meditations', content: 'The happiness of your life depends upon the quality of your thoughts.' }
        ],
        posts: [
          { title: 'Chào mừng năm mới 2026', excerpt: 'Một khởi đầu mới cho những hành trình mới...', content: 'Năm 2026 hứa hẹn sẽ là một năm bùng nổ của công nghệ và sự sáng tạo. Chúng ta hãy cùng nhau xây dựng những thói quen tốt và đạt được những mục tiêu lớn lao.', author: 'Huy', date: '2026-01-01' },
          { title: 'Kế hoạch đọc sách 2026', excerpt: 'Danh sách 52 cuốn sách cho 52 tuần...', content: 'Tôi đã lên danh sách chi tiết các cuốn sách cần đọc trong năm nay. Tập trung vào: Triết học khắc kỷ, Tâm lý học hành vi và Kỹ năng lãnh đạo.', author: 'Huy', date: '2026-01-01' },
          { title: 'Tối ưu hóa Workspace', excerpt: 'Setup lại góc làm việc để tăng 200% năng suất...', content: 'Một không gian làm việc sạch sẽ và tối giản là chìa khóa để tập trung sâu. Tôi đã loại bỏ mọi thứ không cần thiết.', author: 'Huy', date: '2026-01-01' }
        ]
      },
      { 
        date: new Date(2026, 0, 4), 
        label: 'CN', 
        color: '#4caf50', 
        dots: ['#2196f3'], 
        type: 'mood',
        posts: [
          { title: 'Tư duy hệ thống', excerpt: 'Cách nhìn nhận thế giới qua các mối liên hệ...', content: 'Tư duy hệ thống giúp chúng ta hiểu được sự phức tạp của thực tại và đưa ra những quyết định sáng suốt hơn. Nó không chỉ là công cụ, mà là một lăng kính mới để nhìn đời.', author: 'Huy', date: '2026-01-04' },
          { title: 'Review tuần 1', excerpt: 'Những bài học rút ra sau 7 ngày đầu năm...', content: 'Tuần đầu tiên trôi qua khá ổn. Tôi đã duy trì được việc dậy sớm và viết memento mỗi ngày. Cần tập trung hơn vào việc tập thể dục.', author: 'Huy', date: '2026-01-04' }
        ]
      },
      { 
        date: new Date(2026, 0, 8), 
        label: 'T5', 
        color: '#ff9800', 
        dots: ['#ff9800', '#e91e63'], 
        type: 'idea',
        books: [
          { title: 'Atomic Habits', content: 'Small changes, remarkable results. Build systems that work for you.' }
        ]
      },
      { date: new Date(2026, 0, 10), label: 'T7', color: '#e91e63', dots: ['#e91e63'], type: 'mood' },
      { date: new Date(2026, 0, 11), label: 'CN', color: '#e91e63', dots: ['#ff9800'], type: 'idea' },
      { 
        date: new Date(2026, 0, 12), 
        label: 'T2', 
        color: '#4caf50', 
        dots: ['#4caf50'], 
        type: 'all',
        books: [{ title: 'Deep Work', content: 'Rules for focused success in a distracted world.' }]
      },
      { date: new Date(2026, 0, 14), label: 'T4', color: '#4caf50', dots: ['#4caf50'], type: 'mood' },
      { 
        date: new Date(2026, 0, 15), 
        label: 'CN', 
        color: '#ff9800', 
        dots: ['#2196f3'], 
        type: 'mood',
        books: [{ title: 'Man Search for Meaning', content: 'Finding meaning in suffering.' }]
      },
      { date: new Date(2026, 0, 16), label: 'T2', color: '#e91e63', dots: ['#ff9800'], type: 'idea' },
      { 
        date: new Date(2026, 0, 20), 
        label: 'T3', 
        color: '#4caf50', 
        dots: ['#ff9800', '#4caf50'], 
        type: 'all',
        books: [{ title: 'The War of Art', content: 'Break through the blocks and win your inner creative battles.' }]
      },
      { date: new Date(2026, 0, 22), label: 'T5', color: '#4caf50', dots: ['#4caf50'], type: 'mood' },
      { 
        date: new Date(2026, 0, 31), 
        label: 'T7', 
        color: '#4caf50', 
        dots: ['#9c27b0'], 
        type: 'idea',
        books: [{ title: 'Thinking, Fast and Slow', content: 'Two systems of thought.' }]
      },

      // February
      { date: new Date(2026, 1, 4), label: 'T4', color: '#ff9800', dots: ['#ff9800'], type: 'idea' },
      { 
        date: new Date(2026, 1, 10), 
        label: 'T3', 
        color: '#4caf50', 
        dots: ['#4caf50'], 
        type: 'all',
        books: [{ title: 'Grit', content: 'The power of passion and perseverance.' }],
        posts: [
          { title: 'Kỷ luật tự thân', excerpt: 'Tại sao kỷ luật quan trọng hơn động lực...', content: 'Động lực là thứ giúp bạn bắt đầu, nhưng kỷ luật mới là thứ giúp bạn tiếp tục. Hãy xây dựng những routine vững chắc.', author: 'Huy', date: '2026-02-10' }
        ]
      },
      { date: new Date(2026, 1, 11), label: 'T4', color: '#ff9800', dots: ['#ff9800'], type: 'mood' },
      { 
        date: new Date(2026, 1, 14), 
        label: 'T7', 
        color: '#e91e63', 
        dots: ['#e91e63'], 
        type: 'mood',
        books: [{ title: 'The Alchemist', content: 'Follow your dreams.' }],
        posts: [
          { title: 'Lễ tình nhân và sự cô độc', excerpt: 'Suy ngẫm về tình yêu và bản thân...', content: 'Tình yêu lớn nhất là tình yêu dành cho chính mình. Hãy trân trọng những khoảnh khắc tĩnh lặng.', author: 'Huy', date: '2026-02-14' }
        ]
      },
      { date: new Date(2026, 1, 15), label: 'CN', color: '#e91e63', dots: ['#2196f3'], type: 'mood' },
      { date: new Date(2026, 1, 16), label: 'T2', color: '#e91e63', dots: ['#ff9800'], type: 'idea' },
      { date: new Date(2026, 1, 27), label: 'T6', color: '#4caf50', dots: ['#2196f3'], type: 'mood' },

      // March
      { date: new Date(2026, 2, 1), label: 'CN', color: '#4caf50', dots: ['#4caf50'], type: 'all' },
      { date: new Date(2026, 2, 4), label: 'T4', color: '#ff9800', dots: ['#ff9800'], type: 'idea' },
      { date: new Date(2026, 2, 7), label: 'T7', color: '#ff9800', dots: ['#e91e63'], type: 'mood' },
      { 
        date: new Date(2026, 2, 14), 
        label: 'T7', 
        color: '#e91e63', 
        dots: ['#9c27b0', '#4caf50'], 
        type: 'all',
        posts: [
          { title: 'Project X Update', excerpt: 'Tiến độ dự án quan trọng nhất tháng 3...', content: 'Chúng ta đã đạt được 80% milestone. Cần tập trung vào việc tối ưu hóa backend trong tuần tới.', author: 'Huy', date: '2026-03-14' },
          { title: 'Học TypeScript nâng cao', excerpt: 'Generics và Utility Types...', content: 'TypeScript giúp code an toàn và dễ hiểu hơn nhiều. Tôi đang nghiên cứu sâu về Conditional Types.', author: 'Huy', date: '2026-03-14' }
        ]
      },
      { date: new Date(2026, 2, 16), label: 'T2', color: '#e91e63', dots: ['#ff9800'], type: 'idea' },
      { date: new Date(2026, 2, 18), label: 'T4', color: '#e91e63', dots: ['#9c27b0', '#2196f3'], type: 'mood' },
      { date: new Date(2026, 2, 19), label: 'T5', color: '#4caf50', dots: ['#9c27b0'], type: 'idea' },
      { date: new Date(2026, 2, 23), label: 'T2', color: '#4caf50', dots: ['#e91e63'], type: 'mood' },
      { date: new Date(2026, 2, 24), label: 'T3', color: '#4caf50', dots: ['#ff9800', '#2196f3'], type: 'all' },
      { date: new Date(2026, 2, 25), label: 'T4', color: '#4caf50', dots: ['#2196f3'], type: 'mood' },
      { date: new Date(2026, 2, 31), label: 'T3', color: '#e91e63', dots: ['#2196f3'], type: 'mood' },

      // April
      { date: new Date(2026, 3, 6), label: 'T2', color: '#4caf50', dots: ['#e91e63', '#2196f3'], type: 'all' },
      { date: new Date(2026, 3, 10), label: 'T6', color: '#4caf50', dots: ['#9c27b0'], type: 'idea' },
      { date: new Date(2026, 3, 13), label: 'T2', color: '#4caf50', dots: ['#9c27b0'], type: 'idea' },
      { date: new Date(2026, 3, 16), label: 'T5', color: '#4caf50', dots: ['#ff9800'], type: 'idea' },
      { date: new Date(2026, 3, 17), label: 'T6', color: '#4caf50', dots: ['#2196f3'], type: 'mood' },
      { date: new Date(2026, 3, 18), label: 'T7', color: '#4caf50', dots: ['#e91e63'], type: 'mood' },
      { date: new Date(2026, 3, 28), label: 'T3', color: '#4caf50', dots: ['#ff9800'], type: 'idea' },

      // May
      { date: new Date(2026, 4, 5), label: 'T3', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Quiet', content: 'The power of introverts in a world that can\'t stop talking.' }] },
      { date: new Date(2026, 4, 12), label: 'T3', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Think Again', content: 'The power of knowing what you don\'t know.' }] },
      { date: new Date(2026, 4, 15), label: 'T6', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'Originals', content: 'How non-conformists move the world.' }] },
      { date: new Date(2026, 4, 25), label: 'T2', color: '#e91e63', dots: [], type: 'all', books: [{ title: 'Blink', content: 'The power of thinking without thinking.' }] },

      // June
      { date: new Date(2026, 5, 5), label: 'T5', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Switch', content: 'How to change things when change is hard.' }] },
      { date: new Date(2026, 5, 10), label: 'T4', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Outliers', content: 'The story of success.' }] },
      { date: new Date(2026, 5, 20), label: 'T7', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'Flow', content: 'The psychology of optimal experience.' }] },
      { date: new Date(2026, 5, 30), label: 'T3', color: '#e91e63', dots: [], type: 'all', books: [{ title: 'Drive', content: 'The surprising truth about what motivates us.' }] },

      // July
      { date: new Date(2026, 6, 7), label: 'T3', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Sapiens', content: 'A brief history of humankind.' }] },
      { date: new Date(2026, 6, 12), label: 'T1', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Never Split the Difference', content: 'Negotiating as if your life depended on it.' }] },
      { date: new Date(2026, 6, 17), label: 'T6', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'Homo Deus', content: 'A brief history of tomorrow.' }] },
      { date: new Date(2026, 6, 27), label: 'T1', color: '#e91e63', dots: [], type: 'all', books: [{ title: '21 Lessons for the 21st Century', content: 'Navigating the modern world.' }] },

      // August
      { date: new Date(2026, 7, 5), label: 'T3', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Factfulness', content: 'Ten reasons we are wrong about the world.' }] },
      { date: new Date(2026, 7, 10), label: 'T1', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Range', content: 'Why generalists triumph in a specialized world.' }] },
      { date: new Date(2026, 7, 15), label: 'T7', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'Enlightenment Now', content: 'The case for reason, science, humanism, and progress.' }] },
      { date: new Date(2026, 7, 25), label: 'T3', color: '#e91e63', dots: [], type: 'all', books: [{ title: 'Zero to One', content: 'Notes on startups, or how to build the future.' }] },

      // September
      { date: new Date(2026, 8, 5), label: 'T6', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'The E-Myth Revisited', content: 'Why most small businesses don\'t work.' }] },
      { date: new Date(2026, 8, 10), label: 'T4', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'The Lean Startup', content: 'How today\'s entrepreneurs use continuous innovation.' }] },
      { date: new Date(2026, 8, 20), label: 'T7', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'Good to Great', content: 'Why some companies make the leap... and others don\'t.' }] },
      { date: new Date(2026, 8, 30), label: 'T3', color: '#e91e63', dots: [], type: 'all', books: [{ title: 'Built to Last', content: 'Successful habits of visionary companies.' }] },

      // October
      { date: new Date(2026, 9, 8), label: 'T4', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Extreme Ownership', content: 'How U.S. Navy SEALs lead and win.' }] },
      { date: new Date(2026, 9, 12), label: 'T1', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Principles', content: 'Life and work principles by Ray Dalio.' }] },
      { date: new Date(2026, 9, 18), label: 'T7', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'The Obstacle Is the Way', content: 'The timeless art of turning trials into triumph.' }] },
      { date: new Date(2026, 9, 28), label: 'T3', color: '#e91e63', dots: [], type: 'all', books: [{ title: 'Ego Is the Enemy', content: 'The fight to master our greatest opponent.' }] },

      // November
      { date: new Date(2026, 10, 5), label: 'T4', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Stillness Is the Key', content: 'The path to peace, productivity, and powers.' }] },
      { date: new Date(2026, 10, 10), label: 'T2', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Indistractable', content: 'How to control your attention and choose your life.' }] },
      { date: new Date(2026, 10, 15), label: 'T7', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'The 48 Laws of Power', content: 'Strategies for gaining power.' }] },
      { date: new Date(2026, 10, 25), label: 'T3', color: '#e91e63', dots: [], type: 'all', books: [{ title: 'Mastery', content: 'The path to excellence.' }] },

      // December
      { date: new Date(2026, 11, 5), label: 'T6', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'Show Your Work!', content: '10 ways to share your creativity.' }] },
      { date: new Date(2026, 11, 10), label: 'T4', color: '#4caf50', dots: [], type: 'all', books: [{ title: 'The Laws of Human Nature', content: 'Understanding our behavior.' }] },
      { date: new Date(2026, 11, 20), label: 'T7', color: '#ff9800', dots: [], type: 'all', books: [{ title: 'Man\'s Search for Meaning', content: 'Classic work on finding purpose.' }] },
      { date: new Date(2026, 11, 31), label: 'T4', color: '#e91e63', dots: [], type: 'all', books: [{ title: 'The Power of Now', content: 'A guide to spiritual enlightenment.' }] },
    ];
    return entries;
  }
}
