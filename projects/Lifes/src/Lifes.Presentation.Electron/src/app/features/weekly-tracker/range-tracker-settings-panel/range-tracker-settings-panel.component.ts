import { Component, inject, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoodTrackerService } from '../weekly-tracker.service';
import { PALETTES, PatternAidSettings } from '../../../models/weekly-tracker.model';

@Component({
  selector: 'app-range-tracker-settings-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './range-tracker-settings-panel.component.html',
  styleUrl: './range-tracker-settings-panel.component.css'
})
export class RangeTrackerSettingsPanelComponent {
  protected service = inject(MoodTrackerService);
  close = output<void>();

  protected palettes = PALETTES;

  protected setPalette(id: string) {
    this.service.palette.set(id);
  }

  protected toggleAid(key: keyof PatternAidSettings) {
    this.service.patternAids.update(s => ({ ...s, [key]: !s[key] }));
  }

  protected toggleCompact() {
    this.service.compactRows.update(v => !v);
  }
}
