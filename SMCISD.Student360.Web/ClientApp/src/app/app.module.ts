import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { NgbModule, NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap';
import { SocialLoginModule, AuthServiceConfig, GoogleLoginProvider } from "angularx-social-login";
import { JwtInterceptor } from './_infrastructure/jwt.interceptor';
import { AppRoutingModule } from './app.routing';
import { DataGridComponent } from './components/data-grid/data-grid.component';
import { FilterComponent } from './components/filter/filter.component';
import { LoaderComponent } from './components/loader/loader.component';
import { ChartsModule } from 'ng2-charts';
import { StudentProfileComponent } from './components/studentProfile/student-profile.component';
import { StringShortenPipe } from './components/pipes/string-shorten.pipe';
import { FilterBooleanComponent } from './components/filter-boolean/filter-boolean.component';
import { ReportViewerComponent } from './components/report-viewer/report-viewer.component';
import { ReportViewerModule } from 'ngx-ssrs-reportviewer';
import { Ng5SliderModule } from 'ng5-slider';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { NavMenuComponent } from '../pages/nav-menu/nav-menu.component';
import { HomeComponent } from '../pages/home/home.component';
import { DataTableExampleComponent } from '../pages/home/table-data/data-table-example.component';
import { LoginComponent } from '../pages/login/login.component';
import { LineChartExampleComponent } from '../pages/home/line-chart-example/line-chart-example.component';
import { DnaChartComponent } from '../pages/home/dna-chart/dna-chart.component';
import { ReportListComponent } from '../pages/home/reports/report-list.component';
import { SideNavComponent } from '../pages/side-nav/side-nav.component';
import { YtdCampusComponent } from '../pages/reports/ytd-campus/ytd-campus.component';
import { YtdGradeLevelComponent } from '../pages/reports/ytd-grade-level/ytd-grade-level.component';
import { YtdLevelComponent } from '../pages/reports/ytd-level/ytd-level.component';
import { FilterRangeComponent } from './components/filter-range/filter-range.component';
import { AttendanceModalComponent } from './components/modals/attendance-modal/attendance-modal.component';
import { StudentExtraHoursComponent } from '../pages/home/student-extra-hours/student-extra-hours.component';
import { MessageModalComponent } from './components/modals/message-modal/message-modal.component';
import { StudentHourHistoryComponent } from './components/student-hour-history/student-hour-history.component';
import { AttendanceDetailModalComponent } from './components/modals/attendance-detail-modal/attendance-detail-modal.component';
import { DatePipe } from '@angular/common';
import { StudentCourseTranscriptComponent } from './components/student-course-transcript/student-course-transcript.component';
import { BoldReportViewerModule } from '@boldreports/angular-reporting-components';
import { BoldReportViewerComponent } from './components/bold-report-viewer/bold-report-viewer.component';

// Styles
import '@boldreports/javascript-reporting-controls/Content/material/bold.reports.all.min.css'

// Report viewer
import '@boldreports/javascript-reporting-controls/Scripts/bold.report-viewer.min';

// data-visualization
import '@boldreports/javascript-reporting-controls/Scripts/data-visualization/ej.bulletgraph.min';
import '@boldreports/javascript-reporting-controls/Scripts/data-visualization/ej.chart.min';
import '@boldreports/javascript-reporting-controls/Scripts/data-visualization/ej.map.min';
import { FilterAutocompleteComponent } from './components/filter-autocomplete/filter-autocomplete.component';
import { AddStudentExtraHoursModalComponent } from './components/modals/add-student-extra-hour/add-student-extra-hours-modal.component';

let config = new AuthServiceConfig([
  {
    id: GoogleLoginProvider.PROVIDER_ID,
    provider: new GoogleLoginProvider("318016935719-9tm80lqbk0vkgc8tg324o7srr1hut10p.apps.googleusercontent.com")
  }
]);

export function provideConfig() {
  return config;
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginComponent,
    DataTableExampleComponent,
    DataGridComponent,
    FilterComponent,
    FilterBooleanComponent,
    LoaderComponent,
    LineChartExampleComponent,
    DnaChartComponent,
    StudentProfileComponent,
    StringShortenPipe,
    ReportListComponent,
    ReportViewerComponent,
    StringShortenPipe,
    SideNavComponent,
    FilterRangeComponent,
    YtdCampusComponent,
    YtdGradeLevelComponent,
    YtdLevelComponent,
    AttendanceModalComponent,
    MessageModalComponent,
    StudentExtraHoursComponent,
    StudentHourHistoryComponent,
    AttendanceDetailModalComponent,
    StudentCourseTranscriptComponent,
    BoldReportViewerComponent,
    FilterAutocompleteComponent,
    AddStudentExtraHoursModalComponent
  ],
  entryComponents: [AttendanceModalComponent, MessageModalComponent, AttendanceDetailModalComponent, AddStudentExtraHoursModalComponent],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BoldReportViewerModule,
    HttpClientModule,
    FormsModule,
    NgbModule,
    SocialLoginModule,
    AppRoutingModule,
    ChartsModule,
    Ng5SliderModule,
    ReportViewerModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot()
  ],
  providers: [
    { provide: AuthServiceConfig, useFactory: provideConfig },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    // { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    DatePipe
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
