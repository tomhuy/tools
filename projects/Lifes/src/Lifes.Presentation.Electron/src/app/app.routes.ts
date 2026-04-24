import { Routes } from '@angular/router';
import { SprintBoardComponent } from './features/sprint-board/sprint-board.component';
import { MonthlyCalendarPageComponent } from './features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component';

export const routes: Routes = [
  { path: 'sprint-board', component: SprintBoardComponent },
  { path: 'monthly-calendar', component: MonthlyCalendarPageComponent },
  { path: '', redirectTo: 'monthly-calendar', pathMatch: 'full' }
];
