import { Routes } from '@angular/router';
import { SprintBoardComponent } from './features/sprint-board/sprint-board.component';
import { MonthlyCalendarPageComponent } from './features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component';

import { MementoManagementComponent } from './features/memento-management/memento-management.component';
import { ViewChartPageComponent } from './features/view-chart/view-chart-page.component';

export const routes: Routes = [
  { path: 'sprint-board', component: SprintBoardComponent },
  { path: 'monthly-calendar', component: MonthlyCalendarPageComponent },
  { path: 'memento-management', component: MementoManagementComponent },
  { path: 'view-chart', component: ViewChartPageComponent },
  { path: '', redirectTo: 'monthly-calendar', pathMatch: 'full' }
];
