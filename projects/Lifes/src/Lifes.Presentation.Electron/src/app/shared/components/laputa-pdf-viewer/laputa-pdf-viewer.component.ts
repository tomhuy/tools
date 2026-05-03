import { Component, Input, Output, EventEmitter, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxExtendedPdfViewerModule, NgxExtendedPdfViewerService } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-laputa-pdf-viewer',
  standalone: true,
  imports: [CommonModule, NgxExtendedPdfViewerModule],
  templateUrl: './laputa-pdf-viewer.component.html',
  styleUrl: './laputa-pdf-viewer.component.css',
  providers: [NgxExtendedPdfViewerService]
})
export class LaputaPdfViewerComponent implements AfterViewInit, OnDestroy {
  @Input() pdfSrc: string | Uint8Array | null = null;
  @Input() showToolbar: boolean = false;
  @Input() showSidebar: boolean = false;
  @Input() theme: 'dark' | 'light' = 'dark';
  @Input() annotationsData: string | null = null;

  @Output() onTextSelected = new EventEmitter<{ text: string, rect: DOMRect }>();
  @Output() onAnnotationAdded = new EventEmitter<any>();

  private mouseUpListener: (e: MouseEvent) => void;

  constructor(
    private pdfService: NgxExtendedPdfViewerService,
    private elRef: ElementRef
  ) {
    this.mouseUpListener = this.handleMouseUp.bind(this);
  }

  ngAfterViewInit() {
    // Attach event listener to capture text selections over the PDF container
    this.elRef.nativeElement.addEventListener('mouseup', this.mouseUpListener);
  }

  ngOnDestroy() {
    this.elRef.nativeElement.removeEventListener('mouseup', this.mouseUpListener);
  }

  public async onPdfLoaded() {
    // Inject previously saved annotations when the PDF finishes loading
    if (this.annotationsData) {
      try {
        const annotations = JSON.parse(this.annotationsData);
        if (Array.isArray(annotations)) {
          annotations.forEach(anno => {
            this.pdfService.addEditorAnnotation(anno);
          });
        }
      } catch (e) {
        console.error('Failed to load PDF annotations:', e);
      }
    }
  }

  public onAnnotationEditorSave(event: any) {
    // Bubble up the annotation event
    this.onAnnotationAdded.emit(event);
  }

  private handleMouseUp(event: MouseEvent) {
    const selection = window.getSelection();
    if (selection && selection.toString().trim().length > 0) {
      // Ensure the selection is within our component
      if (this.elRef.nativeElement.contains(selection.anchorNode)) {
        const range = selection.getRangeAt(0);
        const rect = range.getBoundingClientRect();
        
        this.onTextSelected.emit({
          text: selection.toString().trim(),
          rect: rect
        });
      }
    }
  }
}
