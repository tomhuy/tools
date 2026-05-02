import { Component, ElementRef, HostListener, ViewChild, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PdfReaderService } from '../services/pdf-reader.service';

@Component({
  selector: 'app-pdf-reader-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pdf-reader-page.component.html',
  styleUrls: ['./pdf-reader-page.component.css']
})
export class PdfReaderPageComponent {
  private pdfReaderService = inject(PdfReaderService);

  // State Signals
  layout = signal<number>(1);
  zoomLevel = signal<number>(100);
  isDark = signal<boolean>(false);
  l2BooksOpen = signal<boolean>(false);
  l2NotesOpen = signal<boolean>(false);
  
  // Data Signals
  books = this.pdfReaderService.books;
  notes = this.pdfReaderService.notes;
  tocItems = this.pdfReaderService.tocItems;

  // New Note State
  newNoteText = signal<string>('');

  // Selection state
  savedRange: Range | null = null;
  selAnchorX = signal<number>(0);
  selAnchorY = signal<number>(0);
  isToolbarVisible = signal<boolean>(false);
  isNotePopupVisible = signal<boolean>(false);
  noteHL = signal<'amber' | 'purple' | 'green' | 'none'>('amber');
  snpQuoteText = signal<string>('');

  @ViewChild('selToolbar') selToolbarRef!: ElementRef;
  @ViewChild('selNotePopup') selNotePopupRef!: ElementRef;

  constructor() {}

  setLayout(n: number) {
    this.layout.set(n);
  }

  changeZoom(delta: number) {
    const current = this.zoomLevel();
    const newZoom = Math.max(60, Math.min(200, current + delta));
    this.zoomLevel.set(newZoom);
  }

  toggleDark() {
    this.isDark.update(v => !v);
  }

  toggleL2Books() {
    this.l2BooksOpen.update(v => !v);
    if (this.l2BooksOpen() && this.l2NotesOpen()) {
      this.l2NotesOpen.set(false);
    }
  }

  toggleL2Notes() {
    this.l2NotesOpen.update(v => !v);
    if (this.l2NotesOpen() && this.l2BooksOpen()) {
      this.l2BooksOpen.set(false);
    }
  }

  onPageInputChange(event: any) {
    // Only visual update, actual pagination logic not fully implemented yet
    let v = Math.max(1, Math.min(320, parseInt(event.target.value) || 1));
    event.target.value = v;
  }

  // Selection & Highlight Logic
  @HostListener('document:mouseup', ['$event'])
  onMouseUp(event: MouseEvent) {
    if (this.isToolbarVisible() && this.selToolbarRef?.nativeElement.contains(event.target)) return;
    if (this.isNotePopupVisible() && this.selNotePopupRef?.nativeElement.contains(event.target)) return;

    setTimeout(() => {
      const sel = window.getSelection();
      const txt = sel ? sel.toString().trim() : '';

      if (!txt || txt.length < 2) {
        this.hideToolbar();
        return;
      }

      const range = sel!.getRangeAt(0);
      const container = range.commonAncestorContainer;
      const element = container.nodeType === 3 ? container.parentElement : container as HTMLElement;
      const page = element?.closest('.pdf-page');
      
      if (!page) { 
        this.hideToolbar(); 
        return; 
      }

      this.savedRange = range.cloneRange();
      const rect = range.getBoundingClientRect();
      
      this.selAnchorX.set(rect.left + rect.width / 2);
      this.selAnchorY.set(rect.top);

      this.isToolbarVisible.set(true);

      // Positioning logic is handled in template with ngStyle or handled here via ViewChild if needed.
      // For simplicity, we calculate directly in template or after view init.
      setTimeout(() => {
        if (!this.selToolbarRef) return;
        const tbw = this.selToolbarRef.nativeElement.offsetWidth;
        const tbh = this.selToolbarRef.nativeElement.offsetHeight;
        let left = this.selAnchorX() - tbw / 2;
        let top = this.selAnchorY() - tbh - 8;
        left = Math.max(8, Math.min(window.innerWidth - tbw - 8, left));
        top = Math.max(52, top);
        
        this.selToolbarRef.nativeElement.style.left = left + 'px';
        this.selToolbarRef.nativeElement.style.top = top + 'px';
      }, 0);
    }, 10);
  }

  @HostListener('document:mousedown', ['$event'])
  onMouseDown(event: MouseEvent) {
    const isToolbar = this.selToolbarRef?.nativeElement.contains(event.target);
    const isPopup = this.selNotePopupRef?.nativeElement.contains(event.target);
    if (!isToolbar && !isPopup) {
      this.hideToolbar();
      this.closeNotePopup();
    }
  }

  hideToolbar() {
    this.isToolbarVisible.set(false);
  }

  applyHL(color: 'amber' | 'purple' | 'green') {
    if (!this.savedRange) return;
    this.wrapRangeWithHL(this.savedRange, color, '');
    this.hideToolbar();
    window.getSelection()?.removeAllRanges();
    this.savedRange = null;
  }

  wrapRangeWithHL(range: Range, color: 'amber' | 'purple' | 'green' | 'none', noteText: string) {
    if (color === 'none') {
      if (noteText) {
         this.addNoteToPanel(range.toString(), noteText, 'amber');
      }
      return;
    }

    const hlColors: any = {
      amber: { bg: '#FFF3D6', darkBg: '#332800', dot: '#EF9F27' },
      purple: { bg: '#EDE9FF', darkBg: '#1E1A3A', dot: '#7F77DD' },
      green: { bg: '#E1F5EE', darkBg: '#0A2A1E', dot: '#1D9E75' }
    };
    const c = hlColors[color];
    const span = document.createElement('span');
    span.className = 'hl-saved';
    span.style.background = this.isDark() ? c.darkBg : c.bg;
    span.dataset['color'] = color;
    span.dataset['note'] = noteText;
    span.title = noteText || '';
    if (noteText) {
      span.style.borderBottom = '2px solid ' + c.dot;
      span.style.paddingBottom = '1px';
    }

    try {
      range.surroundContents(span);
      if (noteText) this.addNoteToPanel(range.toString() || span.textContent || '', noteText, color);
    } catch(ex) {
      const frag = range.extractContents();
      span.appendChild(frag);
      range.insertNode(span);
      if (noteText) this.addNoteToPanel(span.textContent || '', noteText, color);
    }
  }

  addNoteToPanel(quote: string, noteText: string, color: string) {
    this.pdfReaderService.addNote({
      colorType: color as any,
      colorHex: `var(--${color === 'purple' ? 'accent2' : (color === 'green' ? 'accent' : 'amber')})`,
      quote: quote.trim(),
      text: noteText,
      book: 'Atlas of the Heart',
      page: 47, // static for now
      timestamp: new Date().toISOString().slice(0, 16).replace('T', ' ')
    });
  }

  openNotePopup() {
    if (!this.savedRange) return;
    const txt = this.savedRange.toString().trim();
    this.snpQuoteText.set('"' + txt.slice(0, 80) + (txt.length > 80 ? '…' : '') + '"');
    this.newNoteText.set('');
    this.setNoteHL('amber');
    this.hideToolbar();

    this.isNotePopupVisible.set(true);

    setTimeout(() => {
      if (!this.selNotePopupRef) return;
      const pw = this.selNotePopupRef.nativeElement.offsetWidth;
      const ph = this.selNotePopupRef.nativeElement.offsetHeight;
      let left = this.selAnchorX() - pw / 2;
      let top = this.selAnchorY() - ph - 10;
      left = Math.max(8, Math.min(window.innerWidth - pw - 8, left));
      if (top < 52) top = this.selAnchorY() + 24; 
      
      this.selNotePopupRef.nativeElement.style.left = left + 'px';
      this.selNotePopupRef.nativeElement.style.top = top + 'px';
    }, 0);
  }

  closeNotePopup() {
    this.isNotePopupVisible.set(false);
  }

  setNoteHL(color: 'amber' | 'purple' | 'green' | 'none') {
    this.noteHL.set(color);
  }

  saveNote() {
    const noteText = this.newNoteText().trim();
    if (!this.savedRange) { this.closeNotePopup(); return; }
    
    if (this.noteHL() !== 'none') {
      this.wrapRangeWithHL(this.savedRange, this.noteHL(), noteText);
    } else if (noteText) {
      this.addNoteToPanel(this.savedRange.toString(), noteText, 'amber');
    }
    
    this.closeNotePopup();
    window.getSelection()?.removeAllRanges();
    this.savedRange = null;
  }

  copySelection() {
    const txt = this.savedRange ? this.savedRange.toString() : '';
    if (txt && navigator.clipboard) navigator.clipboard.writeText(txt);
    this.hideToolbar();
    window.getSelection()?.removeAllRanges();
    this.savedRange = null;
  }

  exportCSV() {
    // Basic CSV export
    let csv = '\\uFEFFMàu,Sách,Trang,Đoạn trích,Ghi chú,Thời gian\\n';
    this.notes().forEach((n: any) => {
      csv += `"${n.colorType}","${n.book}","${n.page}","${n.quote}","${n.text}","${n.timestamp}"\\n`;
    });

    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'notes-export.csv';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }
}
