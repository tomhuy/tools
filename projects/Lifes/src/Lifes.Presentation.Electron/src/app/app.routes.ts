import { Routes } from '@angular/router';
import { SprintBoardComponent } from './features/sprint-board/sprint-board.component';
import { MonthlyCalendarPageComponent } from './features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component';

import { MementoManagementComponent } from './features/memento-management/memento-management.component';

export const routes: Routes = [
  { path: 'sprint-board', component: SprintBoardComponent },
  { path: 'monthly-calendar', component: MonthlyCalendarPageComponent },
  { path: 'memento-management', component: MementoManagementComponent },
  { path: '', redirectTo: 'monthly-calendar', pathMatch: 'full' }
];
