import { Component, inject } from '@angular/core';
import { MoodTrackerService } from '../weekly-tracker.service';
import { ViewMode } from '../../../models/weekly-tracker.model';

@Component({
  selector: 'app-view-selector-bar',
  standalone: true,
  templateUrl: './view-selector-bar.component.html',
  styleUrl: './view-selector-bar.component.css'
})
export class ViewSelectorBarComponent {
  protected service = inject(MoodTrackerService);

  protected setView(mode: ViewMode) {
    this.service.viewMode.set(mode);
  }
}
