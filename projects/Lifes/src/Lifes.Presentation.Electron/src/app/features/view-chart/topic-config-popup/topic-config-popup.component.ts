import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Memento } from '../../../models/memento.model';
import { ChartSetting, LabelValue, ChartType } from '../../../models/chart.model';

@Component({
  selector: 'app-topic-config-popup',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal-overlay" (click)="onCancel()">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <header class="modal-header">
          <h3>Configure: {{ topic.title }}</h3>
          <button class="close-btn" (click)="onCancel()">×</button>
        </header>

        <div class="modal-body">
          <div class="form-group">
            <label>Chart Type</label>
            <select [(ngModel)]="setting.chartType">
              <option value="line">Line Chart</option>
              <option value="bar">Bar Chart</option>
              <option value="scatter">Scatter Plot</option>
            </select>
          </div>

          <div class="form-group">
            <label>Display Order (Vertical Layer)</label>
            <input type="number" [(ngModel)]="setting.order" placeholder="0 = top">
          </div>

          <div class="mapping-section">
            <header class="section-header">
              <h4>Data Mapping (Label → Value)</h4>
              <button class="add-btn" (click)="addMapping()">+ Add</button>
            </header>

            <div class="mapping-grid">
              <div class="grid-header">
                <span>Label</span>
                <span>Value</span>
                <span>Color</span>
                <span></span>
              </div>

              @for (lv of setting.labelValues; track $index) {
                <div class="grid-row">
                  <input type="text" [(ngModel)]="lv.label" placeholder="D, C-, ...">
                  <input type="number" [(ngModel)]="lv.value" placeholder="0">
                  <input type="color" [(ngModel)]="lv.color">
                  <button class="delete-btn" (click)="removeMapping($index)">🗑️</button>
                </div>
              }
            </div>
          </div>
        </div>

        <footer class="modal-footer">
          <button class="btn-secondary" (click)="onCancel()">Cancel</button>
          <button class="btn-primary" (click)="onSave()">Save Changes</button>
        </footer>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0; left: 0; right: 0; bottom: 0;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      backdrop-filter: blur(4px);
    }
    .modal-container {
      background: white;
      width: 500px;
      border-radius: 16px;
      box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1);
      overflow: hidden;
      display: flex;
      flex-direction: column;
    }
    .modal-header {
      padding: 20px;
      border-bottom: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .modal-header h3 { margin: 0; color: #1e293b; }
    .close-btn { background: none; border: none; font-size: 24px; cursor: pointer; color: #94a3b8; }
    
    .modal-body {
      padding: 20px;
      max-height: 70vh;
      overflow-y: auto;
    }
    .form-group { margin-bottom: 20px; }
    .form-group label { display: block; margin-bottom: 8px; font-weight: 500; color: #64748b; font-size: 13px; }
    .form-group select, .form-group input {
      width: 100%;
      padding: 10px;
      border: 1px solid #e2e8f0;
      border-radius: 8px;
      outline: none;
    }
    
    .mapping-section {
      background: #f8fafc;
      padding: 16px;
      border-radius: 12px;
    }
    .section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .section-header h4 { margin: 0; font-size: 14px; color: #475569; }
    
    .mapping-grid { display: flex; flex-direction: column; gap: 8px; }
    .grid-header { display: grid; grid-template-columns: 2fr 1fr 1fr 40px; gap: 8px; font-size: 12px; font-weight: 600; color: #94a3b8; padding: 0 4px; }
    .grid-row { display: grid; grid-template-columns: 2fr 1fr 1fr 40px; gap: 8px; align-items: center; }
    .grid-row input { padding: 6px; border: 1px solid #e2e8f0; border-radius: 6px; font-size: 13px; }
    .grid-row input[type="color"] { padding: 2px; height: 32px; width: 100%; }
    
    .add-btn { background: #0ea5e9; color: white; border: none; padding: 4px 12px; border-radius: 6px; font-size: 12px; cursor: pointer; }
    .delete-btn { background: none; border: none; cursor: pointer; font-size: 16px; }
    
    .modal-footer { padding: 20px; border-top: 1px solid #e2e8f0; display: flex; justify-content: flex-end; gap: 12px; }
    .btn-secondary { background: #f1f5f9; border: none; padding: 10px 20px; border-radius: 8px; cursor: pointer; font-weight: 500; }
    .btn-primary { background: #1e293b; color: white; border: none; padding: 10px 20px; border-radius: 8px; cursor: pointer; font-weight: 500; }
  `]
})
export class TopicConfigPopupComponent {
  @Input() topic!: Memento;
  @Input() initialSetting?: ChartSetting;
  @Output() save = new EventEmitter<ChartSetting>();
  @Output() cancel = new EventEmitter<void>();

  setting: ChartSetting = {
    chartType: 'line',
    labelValues: [],
    order: 0
  };

  ngOnInit() {
    if (this.initialSetting) {
      // Clone to avoid immediate updates
      this.setting = JSON.parse(JSON.stringify(this.initialSetting));
    } else {
      // Default mappings if empty (just a suggestion)
      this.setting.labelValues = [
        { label: 'D', value: 0, color: '#ef4444' },
        { label: 'C-', value: 1, color: '#f97316' },
        { label: 'C', value: 2, color: '#facc15' },
        { label: 'C+', value: 3, color: '#a855f7' },
        { label: 'B-', value: 4, color: '#fbbf24' },
        { label: 'B', value: 5, color: '#22c55e' },
        { label: 'B+', value: 6, color: '#16a34a' }
      ];
    }
  }

  addMapping() {
    this.setting.labelValues.push({ label: '', value: 0, color: '#cbd5e0' });
  }

  removeMapping(index: number) {
    this.setting.labelValues.splice(index, 1);
  }

  onSave() {
    this.save.emit(this.setting);
  }

  onCancel() {
    this.cancel.emit();
  }
}
