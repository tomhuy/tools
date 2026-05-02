import { Component, HostBinding, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LaputaNotesService } from '../services/laputa-notes.service';

import { LaputaSidebarComponent } from '../laputa-sidebar/laputa-sidebar.component';
import { LaputaNoteListComponent } from '../laputa-note-list/laputa-note-list.component';
import { LaputaEditorComponent } from '../laputa-editor/laputa-editor.component';

@Component({
  selector: 'app-laputa-notes-page',
  standalone: true,
  imports: [CommonModule, FormsModule, LaputaSidebarComponent, LaputaNoteListComponent, LaputaEditorComponent],
  templateUrl: './laputa-notes-page.component.html',
  styleUrls: ['./laputa-notes-page.component.css']
})
export class LaputaNotesPageComponent {
  public noteService = inject(LaputaNotesService);

  // Bind the theme class to the host element
  @HostBinding('class.sepia') get isSepia() {
    return this.noteService.theme() === 'sepia';
  }

  // Manage New Note Modal state directly here or in service? 
  // For simplicity and avoiding circular deps, we can keep some UI state in the service or here.
  // Service already has data state, let's keep modal state here for now.
  public isNewNoteModalOpen = false;
  public newNoteTitle = '';
  public selectedTags: string[] = [];

  openNewNoteModal() {
    this.selectedTags = [];
    this.newNoteTitle = '';
    this.isNewNoteModalOpen = true;
  }

  closeNewNoteModal() {
    this.isNewNoteModalOpen = false;
  }

  toggleModalTag(tag: string) {
    const idx = this.selectedTags.indexOf(tag);
    if (idx > -1) {
      this.selectedTags.splice(idx, 1);
    } else {
      this.selectedTags.push(tag);
    }
  }

  createNote() {
    this.noteService.addNote(this.newNoteTitle, this.selectedTags);
    this.closeNewNoteModal();
  }
}
