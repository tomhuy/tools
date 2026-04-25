import { Component, inject, signal, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Tag } from '../../../models/tag.model';
import { TagService } from '../tag.service';
import { TAG_COLORS } from '../tag.constants';

@Component({
  selector: 'app-tag-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tag-management.component.html',
  styleUrl: './tag-management.component.css'
})
export class TagManagementComponent {
  private readonly tagService = inject(TagService);

  readonly close = output<void>();

  readonly tags = this.tagService.tags;
  readonly isLoading = this.tagService.isLoading;
  readonly palette = TAG_COLORS;

  readonly editingTag = signal<Tag | null>(null);

  // Form State
  readonly formName = signal('');
  readonly formColor = signal('#3b82f6');

  onAdd() {
    this.resetForm();
  }

  onEdit(tag: Tag) {
    this.editingTag.set(tag);
    this.formName.set(tag.name);
    this.formColor.set(tag.color);
  }

  onDelete(id: number) {
    if (confirm('Bạn có chắc chắn muốn xóa tag này? Các memento liên quan sẽ không còn gắn tag này.')) {
      this.tagService.deleteTag(id);
    }
  }

  onSave() {
    const name = this.formName().trim();
    if (!name) return;

    const tag: Tag = {
      id: this.editingTag()?.id ?? 0,
      name,
      color: this.formColor()
    };

    if (tag.id === 0) {
      this.tagService.addTag(tag);
    } else {
      this.tagService.updateTag(tag);
    }

    this.resetForm();
  }

  onCancel() {
    this.resetForm();
  }

  private resetForm() {
    this.editingTag.set(null);
    this.formName.set('');
    this.formColor.set('#3b82f6');
  }

  selectColor(color: string) {
    this.formColor.set(color);
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close.emit();
    }
  }
}
