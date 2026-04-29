import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { YearlyStreamService } from '../yearly-stream.service';
import { StreamEntry, StreamBook, StreamPost } from '../../../models/yearly-stream.model';

@Component({
  selector: 'app-yearly-stream-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './yearly-stream-page.component.html',
  styleUrl: './yearly-stream-page.component.css'
})
export class YearlyStreamPageComponent {
  private streamService = inject(YearlyStreamService);

  currentYear = this.streamService.year;
  currentFilter = this.streamService.filter;
  entries = this.streamService.filteredEntries;

  months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
  days = Array.from({ length: 31 }, (_, i) => i + 1);

  today = new Date();
  
  selectedBooks = signal<StreamBook[] | null>(null);
  selectedPosts = signal<StreamPost[] | null>(null);
  selectedPostIndex = signal<number>(0);

  openReader(books: StreamBook[]) {
    this.selectedBooks.set(books);
  }

  closeReader() {
    this.selectedBooks.set(null);
  }

  openPostReader(posts: StreamPost[]) {
    this.selectedPosts.set(posts);
    this.selectedPostIndex.set(0); // Default to first post
  }

  closePostReader() {
    this.selectedPosts.set(null);
  }

  selectPost(index: number) {
    this.selectedPostIndex.set(index);
  }
  
  monthSummaries = computed(() => {
    const counts = new Array(12).fill(0);
    this.entries().forEach(e => {
      counts[e.date.getMonth()]++;
    });
    return this.months.map((name, i) => ({
      name,
      count: counts[i] > 0 ? counts[i] : null
    }));
  });

  // Matrix organized by [dayIndex][monthIndex]
  grid = computed(() => {
    const matrix: (StreamEntry | null)[][] = Array.from({ length: 31 }, () => new Array(12).fill(null));
    
    this.entries().forEach(entry => {
      const day = entry.date.getDate() - 1;
      const month = entry.date.getMonth();
      if (day < 31) {
        matrix[day][month] = entry;
      }
    });
    
    return matrix;
  });

  setFilter(filter: 'all' | 'mood' | 'idea') {
    this.streamService.setFilter(filter);
  }

  prevYear() {
    this.streamService.setYear(this.currentYear() - 1);
  }

  nextYear() {
    this.streamService.setYear(this.currentYear() + 1);
  }

  goToday() {
    this.streamService.setYear(this.today.getFullYear());
  }

  isToday(day: number): boolean {
    return this.today.getFullYear() === this.currentYear() &&
           this.today.getDate() === day;
  }

  isCellToday(day: number, monthIdx: number): boolean {
    return this.today.getFullYear() === this.currentYear() &&
           this.today.getMonth() === monthIdx &&
           this.today.getDate() === day;
  }

  isCellFuture(day: number, monthIdx: number): boolean {
    const cellDate = new Date(this.currentYear(), monthIdx, day);
    return cellDate > this.today;
  }

  formatDay(day: number): string {
    return day < 10 ? `0${day}` : `${day}`;
  }
}
