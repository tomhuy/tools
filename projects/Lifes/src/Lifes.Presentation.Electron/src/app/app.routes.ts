import { Routes } from '@angular/router';
import { SprintBoardComponent } from './features/sprint-board/sprint-board.component';
import { MonthlyCalendarPageComponent } from './features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component';

import { MementoManagementComponent } from './features/memento-management/memento-management.component';
import { ViewChartPageComponent } from './features/view-chart/view-chart-page.component';
import { DailyTimelinePageComponent } from './features/daily-timeline/daily-timeline-page/daily-timeline-page.component';
import { YearlyStreamPageComponent } from './features/yearly-stream/yearly-stream-page/yearly-stream-page.component';
import { WeeklyTrackerPageComponent } from './features/weekly-tracker/weekly-tracker-page/weekly-tracker-page.component';
import { RangeTrackerPageComponent } from './features/weekly-tracker/range-tracker-page/range-tracker-page.component';
import { ContentExplorerPageComponent } from './features/weekly-tracker/content-explorer-page/content-explorer-page.component';

export const routes: Routes = [
  { path: 'daily-timeline', component: DailyTimelinePageComponent },
  { path: 'yearly-stream', component: YearlyStreamPageComponent },
  { path: 'weekly-tracker', component: WeeklyTrackerPageComponent },
  { path: 'range-tracker', component: RangeTrackerPageComponent },
  { path: 'content-explorer', component: ContentExplorerPageComponent },
  { path: 'sprint-board', component: SprintBoardComponent },
  { path: 'monthly-calendar', component: MonthlyCalendarPageComponent },
  { path: 'memento-management', component: MementoManagementComponent },
  { path: 'view-chart', component: ViewChartPageComponent },
  { path: '', redirectTo: 'daily-timeline', pathMatch: 'full' }
];
