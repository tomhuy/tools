import { Component, Output, EventEmitter, inject, ElementRef, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaputaNotesService, Note } from '../services/laputa-notes.service';

@Component({
  selector: 'app-laputa-note-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './laputa-note-list.component.html',
  styleUrls: ['./laputa-note-list.component.css']
})
export class LaputaNoteListComponent {
  public noteService = inject(LaputaNotesService);
  
  @Output() openNewNote = new EventEmitter<void>();
  @ViewChild('listPanel') listPanel!: ElementRef;

  public panelWidth = 290;
  private isResizing = false;
  private startX = 0;
  private startW = 0;

  // Context Menu State
  public ctxMenuVisible = false;
  public ctxMenuX = 0;
  public ctxMenuY = 0;
  public ctxTargetId: number | null = null;

  @HostListener('document:mousemove', ['$event'])
  onMouseMove(e: MouseEvent) {
    if (!this.isResizing) return;
    const dx = e.clientX - this.startX;
    this.panelWidth = Math.max(220, Math.min(window.innerWidth * 0.7, this.startW + dx));
    // When width changes, css will handle the grid layout via media queries or inline styles
  }

  @HostListener('document:mouseup')
  onMouseUp() {
    if (!this.isResizing) return;
    this.isResizing = false;
    document.body.style.cursor = '';
    document.body.style.userSelect = '';
  }

  @HostListener('document:click')
  onDocClick() {
    this.ctxMenuVisible = false;
  }

  startResize(e: MouseEvent) {
    this.isResizing = true;
    this.startX = e.clientX;
    this.startW = this.panelWidth;
    document.body.style.cursor = 'col-resize';
    document.body.style.userSelect = 'none';
    e.preventDefault();
  }

  setViewMode(mode: 'list' | 'card' | 'compact' | 'grid') {
    this.noteService.viewMode.set(mode);
    if (mode !== 'grid' && this.noteService.currentNoteId()) {
      // Logic for changing from grid popup to editor is handled in the main page / editor components
    }
  }

  toggleSort() {
    this.noteService.sortAsc.set(!this.noteService.sortAsc());
  }

  getSectionName(): string {
    const sec = this.noteService.currentSection();
    if (sec === 'all') return 'Tất cả';
    if (sec === 'starred') return 'Đánh dấu sao';
    if (sec === 'inbox') return 'Inbox';
    // Ideally find the label from navConfig
    for (const group of this.noteService.navConfig().sections) {
      const item = group.items.find(i => i.id === sec);
      if (item) return item.label;
    }
    return sec;
  }

  openNote(id: number) {
    this.noteService.currentNoteId.set(id);
    this.noteService.isPreview.set(false);
  }

  showCtxMenu(e: MouseEvent, id: number) {
    e.preventDefault();
    this.ctxTargetId = id;
    this.ctxMenuVisible = true;
    this.ctxMenuX = Math.min(e.clientX, window.innerWidth - 180);
    this.ctxMenuY = Math.min(e.clientY, window.innerHeight - 160);
  }

  ctxAction(action: string) {
    if (this.ctxTargetId === null) return;
    
    if (action === 'open') {
      this.openNote(this.ctxTargetId);
    } else if (action === 'duplicate') {
      this.noteService.duplicateNote(this.ctxTargetId);
    } else if (action === 'delete') {
      this.noteService.deleteNote(this.ctxTargetId);
    } else if (action === 'rename') {
      // Just open note and let user focus title (can be enhanced)
      this.openNote(this.ctxTargetId);
    }
    
    this.ctxMenuVisible = false;
  }

  // Helpers for template formatting
  getPreview(content: string): string {
    return content.replace(/[#*`>_\-\d\.]/g, '').trim().substring(0, 110);
  }

  getBullets(content: string): string[] {
    return content.split('\n')
      .map(l => l.replace(/^[#\-*\d\.>\s]+/, '').replace(/\*\*/g, '').replace(/\*/g, '').trim())
      .filter(l => l.length > 8 && l.length < 80)
      .slice(0, 3);
  }

  getAccentColor(section: string | null): string {
    if (!section) return 'var(--text3)';
    const sectionColors: { [key: string]: string } = {
      psychology: 'var(--red)', ideas: 'var(--accent2)', resources: 'var(--green)',
      journal: 'var(--accent)', tasks: 'var(--blue)', work: 'var(--blue)',
      learning: 'var(--accent2)', personal: 'var(--accent)'
    };
    return sectionColors[section] || 'var(--text3)';
  }

  relTime(d: Date): string {
    const diff = Date.now() - d.getTime();
    if (diff < 60000) return 'vừa xong';
    if (diff < 3600000) return Math.floor(diff / 60000) + ' phút trước';
    if (diff < 86400000) return Math.floor(diff / 3600000) + ' giờ trước';
    return Math.floor(diff / 86400000) + ' ngày trước';
  }
}
