import { Routes } from '@angular/router';
import { SprintBoardComponent } from './features/sprint-board/sprint-board.component';
import { MonthlyCalendarPageComponent } from './features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component';

import { MementoManagementComponent } from './features/memento-management/memento-management.component';
import { ViewChartPageComponent } from './features/view-chart/view-chart-page.component';
import { DailyTimelinePageComponent } from './features/daily-timeline/daily-timeline-page/daily-timeline-page.component';
import { YearlyStreamPageComponent } from './features/yearly-stream/yearly-stream-page/yearly-stream-page.component';

export const routes: Routes = [
  { path: 'daily-timeline', component: DailyTimelinePageComponent },
  { path: 'yearly-stream', component: YearlyStreamPageComponent },
  { path: 'sprint-board', component: SprintBoardComponent },
  { path: 'monthly-calendar', component: MonthlyCalendarPageComponent },
  { path: 'memento-management', component: MementoManagementComponent },
  { path: 'view-chart', component: ViewChartPageComponent },
  { path: '', redirectTo: 'daily-timeline', pathMatch: 'full' }
];
