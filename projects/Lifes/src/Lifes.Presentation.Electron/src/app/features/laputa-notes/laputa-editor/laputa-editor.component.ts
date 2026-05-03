import { Component, inject, ViewChild, ElementRef, AfterViewChecked, effect, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { LaputaNotesService } from '../services/laputa-notes.service';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-laputa-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './laputa-editor.component.html',
  styleUrls: ['./laputa-editor.component.css'],
  host: {
    '[class.view-grid]': 'noteService.viewMode() === "grid"',
    '[class.has-note]': 'noteService.currentNoteId() !== null'
  }
})
export class LaputaEditorComponent {
  public noteService = inject(LaputaNotesService);
  private fb = inject(FormBuilder);
  private destroyRef = inject(DestroyRef);

  @ViewChild('noteTitle') noteTitleInput!: ElementRef<HTMLTextAreaElement>;
  @ViewChild('noteContent') noteContentInput!: ElementRef<HTMLTextAreaElement>;
  @ViewChild('dpNoteTitle') dpNoteTitleInput!: ElementRef<HTMLTextAreaElement>;
  @ViewChild('dpNoteContent') dpNoteContentInput!: ElementRef<HTMLTextAreaElement>;

  public noteForm: FormGroup;
  private isPatching = false;

  constructor() {
    this.noteForm = this.fb.group({
      title: [''],
      content: ['']
    });

    // Handle auto-save with debounce
    this.noteForm.valueChanges.pipe(
      debounceTime(200),
      distinctUntilChanged((prev, curr) => prev.title === curr.title && prev.content === curr.content),
      tap(value => {
        if (this.isPatching) return;
        const note = this.noteService.currentNote();
        if (note) {
          this.noteService.saveNote(note.id, value.title, value.content);
        }
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe();

    // Sync form when current note changes
    effect(() => {
      const note = this.noteService.currentNote();
      if (note) {
        // Only patch if values are actually different to prevent scroll reset
        const currentValues = this.noteForm.getRawValue();
        if (currentValues.title !== note.title || currentValues.content !== note.content) {
          this.isPatching = true;
          this.noteForm.patchValue({
            title: note.title,
            content: note.content
          }, { emitEvent: false });
          this.isPatching = false;

          // Trigger auto-resize after patch
          setTimeout(() => {
            if (this.noteContentInput?.nativeElement) this.autoResize(this.noteContentInput.nativeElement);
            if (this.noteTitleInput?.nativeElement) this.autoResize(this.noteTitleInput.nativeElement);
            if (this.dpNoteContentInput?.nativeElement) this.autoResize(this.dpNoteContentInput.nativeElement);
            if (this.dpNoteTitleInput?.nativeElement) this.autoResize(this.dpNoteTitleInput.nativeElement);
          }, 0);
        }
      }
    });
  }

  // Computed words and characters
  get wordCount(): number {
    const content = this.noteForm.get('content')?.value || '';
    if (!content.trim()) return 0;
    return content.trim().split(/\s+/).length;
  }

  get charCount(): number {
    const content = this.noteForm.get('content')?.value || '';
    return content.length;
  }

  autoResize(textarea: HTMLTextAreaElement) {
    if (!textarea) return;
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
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
    const blob = new Blob([this.noteForm.value.content], { type: 'text/markdown' });
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = this.noteForm.value.title.replace(/[^a-z0-9\\u00C0-\\u024F\\s]/gi, '_') + '.md';
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
        const newValue = ta.value.slice(0, lineStart) + '# ' + line.replace(/^#+\\s?/, '') + ta.value.slice(end || lineStart + line.length);
        this.noteForm.patchValue({ content: newValue });
        setTimeout(() => ta.focus(), 0);
        return;
      }
      case 'h2': {
        const line = ta.value.slice(lineStart, end || lineStart + 1);
        const newValue = ta.value.slice(0, lineStart) + '## ' + line.replace(/^#+\\s?/, '') + ta.value.slice(end || lineStart + line.length);
        this.noteForm.patchValue({ content: newValue });
        setTimeout(() => ta.focus(), 0);
        return;
      }
      case 'h3': {
        const line = ta.value.slice(lineStart, end || lineStart + 1);
        const newValue = ta.value.slice(0, lineStart) + '### ' + line.replace(/^#+\\s?/, '') + ta.value.slice(end || lineStart + line.length);
        this.noteForm.patchValue({ content: newValue });
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
    this.noteForm.patchValue({ content: ta.value });
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

  relTime(d: Date | string): string {
    const date = typeof d === 'string' ? new Date(d) : d;
    const diff = Date.now() - date.getTime();
    if (diff < 60000) return 'vừa xong';
    if (diff < 3600000) return Math.floor(diff / 60000) + ' phút trước';
    if (diff < 86400000) return Math.floor(diff / 3600000) + ' giờ trước';
    return Math.floor(diff / 86400000) + ' ngày trước';
  }
}
