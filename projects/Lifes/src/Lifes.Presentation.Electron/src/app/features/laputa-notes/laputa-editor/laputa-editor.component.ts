import { Component, inject, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LaputaNotesService } from '../services/laputa-notes.service';

@Component({
  selector: 'app-laputa-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './laputa-editor.component.html',
  styleUrls: ['./laputa-editor.component.css'],
  host: {
    '[class.view-grid]': 'noteService.viewMode() === "grid"',
    '[class.has-note]': 'noteService.currentNoteId() !== null'
  }
})
export class LaputaEditorComponent implements AfterViewChecked {
  public noteService = inject(LaputaNotesService);
  
  @ViewChild('noteTitle') noteTitleInput!: ElementRef<HTMLTextAreaElement>;
  @ViewChild('noteContent') noteContentInput!: ElementRef<HTMLTextAreaElement>;
  @ViewChild('dpNoteTitle') dpNoteTitleInput!: ElementRef<HTMLTextAreaElement>;
  @ViewChild('dpNoteContent') dpNoteContentInput!: ElementRef<HTMLTextAreaElement>;

  // Computed words and characters
  get wordCount(): number {
    const note = this.noteService.currentNote();
    if (!note || !note.content.trim()) return 0;
    return note.content.trim().split(/\s+/).length;
  }

  get charCount(): number {
    const note = this.noteService.currentNote();
    if (!note) return 0;
    return note.content.length;
  }

  autoResize(textarea: HTMLTextAreaElement) {
    if (!textarea) return;
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
  }

  ngAfterViewChecked() {
    if (this.noteContentInput?.nativeElement) {
      this.autoResize(this.noteContentInput.nativeElement);
    }
    if (this.dpNoteContentInput?.nativeElement) {
      this.autoResize(this.dpNoteContentInput.nativeElement);
    }
  }

  onTitleChange(newTitle: string) {
    const note = this.noteService.currentNote();
    if (!note) return;
    this.noteService.updateNote(note.id, { title: newTitle });
  }

  onContentChange(newContent: string) {
    const note = this.noteService.currentNote();
    if (!note) return;
    this.noteService.updateNote(note.id, { content: newContent });
  }

  togglePreview() {
    this.noteService.isPreview.set(!this.noteService.isPreview());
  }

  closeEditor() {
    this.noteService.currentNoteId.set(null);
  }

  closeDetailPopup() {
    this.noteService.currentNoteId.set(null);
  }

  exportNote() {
    const note = this.noteService.currentNote();
    if (!note) return;
    const blob = new Blob([note.content], { type: 'text/markdown' });
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = note.title.replace(/[^a-z0-9\\u00C0-\\u024F\\s]/gi, '_') + '.md';
    a.click();
  }

  deleteCurrentNote() {
    const id = this.noteService.currentNoteId();
    if (id !== null) {
      this.noteService.deleteNote(id);
    }
  }

  toggleStar() {
    const id = this.noteService.currentNoteId();
    if (id !== null) {
      this.noteService.toggleStar(id);
    }
  }

  mdCmd(cmd: string) {
    if (this.noteService.isPreview()) {
      this.togglePreview();
    }
    const ta = this.noteService.viewMode() === 'grid' 
      ? this.dpNoteContentInput.nativeElement 
      : this.noteContentInput.nativeElement;

    if (!ta) return;

    const start = ta.selectionStart;
    const end = ta.selectionEnd;
    const sel = ta.value.slice(start, end);
    const lineStart = ta.value.lastIndexOf('\n', start - 1) + 1;
    let replacement = '';
    let offset = 0;

    switch (cmd) {
      case 'bold': replacement = `**${sel || 'text'}**`; offset = sel ? 0 : 2; break;
      case 'italic': replacement = `*${sel || 'text'}*`; offset = sel ? 0 : 1; break;
      case 'code': replacement = `\`${sel || 'code'}\``; offset = sel ? 0 : 1; break;
      case 'h1': {
        const line = ta.value.slice(lineStart, end || lineStart + 1);
        ta.value = ta.value.slice(0, lineStart) + '# ' + line.replace(/^#+\\s?/, '') + ta.value.slice(end || lineStart + line.length);
        this.onContentChange(ta.value);
        setTimeout(() => ta.focus(), 0);
        return;
      }
      case 'h2': {
        const line = ta.value.slice(lineStart, end || lineStart + 1);
        ta.value = ta.value.slice(0, lineStart) + '## ' + line.replace(/^#+\\s?/, '') + ta.value.slice(end || lineStart + line.length);
        this.onContentChange(ta.value);
        setTimeout(() => ta.focus(), 0);
        return;
      }
      case 'h3': {
        const line = ta.value.slice(lineStart, end || lineStart + 1);
        ta.value = ta.value.slice(0, lineStart) + '### ' + line.replace(/^#+\\s?/, '') + ta.value.slice(end || lineStart + line.length);
        this.onContentChange(ta.value);
        setTimeout(() => ta.focus(), 0);
        return;
      }
      case 'ul': replacement = `\n- ${sel || 'item'}`; offset = sel ? 0 : 2; break;
      case 'ol': replacement = `\n1. ${sel || 'item'}`; offset = sel ? 0 : 3; break;
      case 'quote': replacement = `\n> ${sel || 'quote'}`; offset = sel ? 0 : 2; break;
      case 'hr': replacement = '\n---\n'; offset = 0; break;
    }

    ta.setRangeText(replacement, start, end, 'end');
    if (!sel) {
      ta.selectionStart = ta.selectionEnd = start + offset + (replacement.length - (sel.length || 0) - offset * 2);
    }
    ta.focus();
    this.autoResize(ta);
    this.onContentChange(ta.value);
  }

  parseMarkdown(md: string): string {
    if (!md) return '';
    return md
      .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
      .replace(/^### (.+)$/gm, '<h3>$1</h3>')
      .replace(/^## (.+)$/gm, '<h2>$1</h2>')
      .replace(/^# (.+)$/gm, '<h1>$1</h1>')
      .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
      .replace(/\*(.+?)\*/g, '<em>$1</em>')
      .replace(/\`([^\`]+)\`/g, '<code>$1</code>')
      .replace(/^> (.+)$/gm, '<blockquote>$1</blockquote>')
      .replace(/^---$/gm, '<hr>')
      .replace(/^\d+\. (.+)$/gm, (m, p) => `<li>${p}</li>`)
      .replace(/^[-*] (.+)$/gm, (m, p) => `<li>${p}</li>`)
      .replace(/(<li>.*<\/li>\n?)+/g, s => `<ul>${s}</ul>`)
      .replace(/\n\n/g, '</p><p>')
      .replace(/^(?!<[hbuolp])(.+)$/gm, '<p>$1</p>')
      .replace(/<p><\/p>/g, '');
  }

  relTime(d: Date): string {
    const diff = Date.now() - d.getTime();
    if (diff < 60000) return 'vừa xong';
    if (diff < 3600000) return Math.floor(diff / 60000) + ' phút trước';
    if (diff < 86400000) return Math.floor(diff / 3600000) + ' giờ trước';
    return Math.floor(diff / 86400000) + ' ngày trước';
  }
}
