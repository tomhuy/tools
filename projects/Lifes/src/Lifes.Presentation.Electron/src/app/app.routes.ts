import { Routes } from '@angular/router';
import { SprintBoardComponent } from './features/sprint-board/sprint-board.component';

export const routes: Routes = [
  { path: 'sprint-board', component: SprintBoardComponent },
  { path: '', redirectTo: 'sprint-board', pathMatch: 'full' }
];
