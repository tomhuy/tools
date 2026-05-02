import { Injectable, signal } from '@angular/core';

export interface Book {
  id: string;
  coverColor: string;
  coverInitials: string;
  title: string;
  author: string;
  progressPercent: number;
  progressPage: number;
}

export interface Note {
  id: string;
  colorType: 'amber' | 'purple' | 'green' | 'none';
  colorHex: string;
  quote: string;
  text: string;
  book: string;
  page: string | number;
  timestamp: string;
}

export interface TocItem {
  id: string;
  title: string;
  isSub: boolean;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PdfReaderService {
  
  // Mock Books
  books = signal<Book[]>([
    { id: '1', coverColor: '#1D9E75', coverInitials: 'AH', title: 'Atlas of the Heart', author: 'Brené Brown', progressPercent: 15, progressPage: 47 },
    { id: '2', coverColor: '#7F77DD', coverInitials: 'SC', title: 'Self-Compassion', author: 'Kristin Neff', progressPercent: 8, progressPage: 12 },
    { id: '3', coverColor: '#EF9F27', coverInitials: 'FL', title: 'Flow', author: 'Csikszentmihalyi', progressPercent: 41, progressPage: 89 },
    { id: '4', coverColor: '#D4537E', coverInitials: 'TF', title: 'Thinking, Fast & Slow', author: 'Daniel Kahneman', progressPercent: 1, progressPage: 3 },
    { id: '5', coverColor: '#378ADD', coverInitials: 'CE', title: 'The Compound Effect', author: 'Darren Hardy', progressPercent: 62, progressPage: 140 }
  ]);

  // Mock Notes
  notes = signal<Note[]>([
    { id: 'n1', colorType: 'amber', colorHex: 'var(--amber)', quote: 'words we use… are not just labels', text: 'Ngôn ngữ là giàn giáo của ý nghĩa cảm xúc', book: 'Atlas of the Heart', page: 47, timestamp: '2026-05-02 00:00' },
    { id: 'n2', colorType: 'purple', colorHex: 'var(--accent2)', quote: 'directly linked to resilience', text: 'Emotion granularity → resilience cao hơn, trầm cảm thấp hơn', book: 'Atlas of the Heart', page: 47, timestamp: '2026-05-02 00:01' },
    { id: 'n3', colorType: 'green', colorHex: 'var(--accent)', quote: '', text: 'Emotion ≠ feeling — phân biệt quan trọng', book: 'Atlas of the Heart', page: 44, timestamp: '2026-05-02 00:02' }
  ]);

  // Mock TOC
  tocItems = signal<TocItem[]>([
    { id: 't1', title: 'Intro — Mapmaking', isSub: false, isActive: false },
    { id: 't2', title: 'Why Language Matters', isSub: true, isActive: false },
    { id: 't3', title: 'Ch.1 — Uncertain Places', isSub: false, isActive: false },
    { id: 't4', title: 'Stress, Anxiety, Fear', isSub: true, isActive: false },
    { id: 't5', title: 'Ch.2 — Comparison', isSub: false, isActive: false },
    { id: 't6', title: 'Ch.3 — Anguish & Language', isSub: false, isActive: true },
    { id: 't7', title: 'The Language of Emotion', isSub: true, isActive: true },
    { id: 't8', title: 'Affect Labeling', isSub: true, isActive: false },
    { id: 't9', title: 'Ch.4 — When Things Go Wrong', isSub: false, isActive: false },
    { id: 't10', title: 'Boredom, Disappointment', isSub: true, isActive: false },
    { id: 't11', title: 'Ch.5 — With Others', isSub: false, isActive: false }
  ]);

  constructor() { }

  addNote(note: Omit<Note, 'id'>) {
    const newNote = { ...note, id: 'n' + Date.now() };
    this.notes.update(notes => [newNote, ...notes]);
  }
}
