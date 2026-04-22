import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';

import { ApiService } from './api.service';
import { ProjectFile } from './models/project.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterOutlet,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  private readonly apiService = inject(ApiService);

  title = 'Lifes Electron Angular';
  
  projects = signal<ProjectFile[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  scanPath = signal<string>('C:\\Users\\bmhuy\\OneDrive\\Desktop\\14 days\\learns\\this-is-my-life\\tools\\projects\\Lifes');

  onScan(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.apiService.scanProjects(this.scanPath()).subscribe({
      next: (data) => {
        this.projects.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set('Failed to connect to Backend API. Make sure the WebAPI is running on http://localhost:5110');
        this.isLoading.set(false);
        console.error(err);
      }
    });
  }
}
