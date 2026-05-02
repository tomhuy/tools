import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LaputaNotesService, NavItem, NavSection } from '../services/laputa-notes.service';

@Component({
  selector: 'app-laputa-sidebar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './laputa-sidebar.component.html',
  styleUrls: ['./laputa-sidebar.component.css']
})
export class LaputaSidebarComponent {
  public noteService = inject(LaputaNotesService);

  getNavSvg(icon: string): string {
    const svgs: { [key: string]: string } = {
      inbox: '<path d="M1 8h3l1.5 2h3L10 8h3V11a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V8z" stroke="currentColor" stroke-width="1.2" fill="none"/><path d="M1 8l2-5h8l2 5" stroke="currentColor" stroke-width="1.2" fill="none"/>',
      all: '<rect x="1" y="1" width="5" height="5" rx="1" stroke="currentColor" stroke-width="1.2" fill="none"/><rect x="8" y="1" width="5" height="5" rx="1" stroke="currentColor" stroke-width="1.2" fill="none"/><rect x="1" y="8" width="5" height="5" rx="1" stroke="currentColor" stroke-width="1.2" fill="none"/><rect x="8" y="8" width="5" height="5" rx="1" stroke="currentColor" stroke-width="1.2" fill="none"/>',
      star: '<path d="M7 1l1.5 4H13l-3.5 2.5 1.5 4L7 9l-4 2.5 1.5-4L1 5h4.5z" stroke="currentColor" stroke-width="1.2" fill="none" stroke-linejoin="round"/>',
    };
    return svgs[icon] || svgs['all'];
  }

  isSvgIcon(icon: string): boolean {
    return icon === 'inbox' || icon === 'all' || icon === 'star';
  }

  getColor(colorStr?: string): string | null {
    return colorStr ? this.noteService.COLORS[colorStr] || null : null;
  }

  toggleSidebar() {
    this.noteService.isSidebarOpen.set(!this.noteService.isSidebarOpen());
  }

  setTheme(theme: 'dark' | 'sepia') {
    this.noteService.theme.set(theme);
  }

  selectSection(id: string) {
    this.noteService.currentSection.set(id);
    // Reset selection and close editor when changing section
    this.noteService.currentNoteId.set(null);
  }

  onSearch(event: any) {
    this.noteService.searchQuery.set(event.target.value);
  }

  addNavItem(sec: NavSection) {
    // Basic prompt implementation, can be refined later
    const label = window.prompt('Tên mục mới:');
    if (!label) return;
    const id = label.toLowerCase().replace(/\\s+/g, '-');
    sec.items.push({ id, label, icon: '📁', color: 'amber' });
  }
}
