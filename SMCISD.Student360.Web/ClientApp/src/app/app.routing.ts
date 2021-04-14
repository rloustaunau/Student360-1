import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from './_infrastructure/auth.guard';
import { NgModule } from '@angular/core';
import { ReportViewerComponent } from './components/report-viewer/report-viewer.component';
import { HomeComponent } from '../pages/home/home.component';
import { DataTableExampleComponent } from '../pages/home/table-data/data-table-example.component';
import { LineChartExampleComponent } from '../pages/home/line-chart-example/line-chart-example.component';
import { DnaChartComponent } from '../pages/home/dna-chart/dna-chart.component';
import { ReportListComponent } from '../pages/home/reports/report-list.component';
import { YtdCampusComponent } from '../pages/reports/ytd-campus/ytd-campus.component';
import { YtdGradeLevelComponent } from '../pages/reports/ytd-grade-level/ytd-grade-level.component';
import { YtdLevelComponent } from '../pages/reports/ytd-level/ytd-level.component';
import { LoginComponent } from '../pages/login/login.component';
import { StudentExtraHoursComponent } from '../pages/home/student-extra-hours/student-extra-hours.component';
import { BoldReportViewerComponent } from './components/bold-report-viewer/bold-report-viewer.component';
import { AttendanceLettersComponent } from 'src/pages/attendance-letters/attendance-letters.component';
import { SecurityLevelComponent } from '../pages/securityLevel/security-level.component';

const routes: Routes = [
  {
    path: '', component: HomeComponent, canActivate: [AuthGuard], children: [
      { path: '', component: DataTableExampleComponent, canActivate: [AuthGuard] },
      { path: 'lineChart', component: LineChartExampleComponent, canActivate: [AuthGuard] },
      { path: 'dnaChart', component: DnaChartComponent, canActivate: [AuthGuard] },
      { path: 'reports', component: ReportListComponent, canActivate: [AuthGuard] },
      { path: 'report/:name', component: BoldReportViewerComponent, canActivate: [AuthGuard] },
      { path: 'ytdcampus', component: YtdCampusComponent, canActivate: [AuthGuard] },
      { path: 'ytdgradelevel', component: YtdGradeLevelComponent, canActivate: [AuthGuard] },
      { path: 'ytdschoollevel', component: YtdLevelComponent, canActivate: [AuthGuard] },
      { path: 'hours/:id', component: StudentExtraHoursComponent, canActivate: [AuthGuard] },
      { path: 'hours', component: StudentExtraHoursComponent, canActivate: [AuthGuard] },
      { path: 'letters', component: AttendanceLettersComponent, canActivate: [AuthGuard] },
      { path: 'securityLevel', component: SecurityLevelComponent, canActivate: [AuthGuard] },
    ]
  },
  { path: 'login', component: LoginComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})

export class AppRoutingModule { }

