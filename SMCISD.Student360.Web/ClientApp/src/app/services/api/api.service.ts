import { Injectable } from '@angular/core';
import { OAuthApiService } from './o-auth-api.service';
import { CurrentUserApiService } from './current-user.service';
import { StudentApiService } from './student.service';
import { SchoolApiService } from './school.service';
import { FilterService } from './filter.service';
import { ReportApiService } from './report.service';
import { ReportViewerService } from './report-viewer.service';
import { ImageApiService } from './image.service';
import { StudentExtraHourService } from './student-extra-hour.service';
import { AttendanceLetterService } from './attendance-letter.service';

@Injectable({ providedIn: 'root' })
export class ApiService {

  constructor(public currentUser: CurrentUserApiService,
    public oauth: OAuthApiService,
    public student: StudentApiService,
    public school: SchoolApiService,
    public filter: FilterService,
    public report: ReportApiService,
    public reportViewer: ReportViewerService,
    public image: ImageApiService,
    public studentExtraHour: StudentExtraHourService,
    public letter: AttendanceLetterService) { }
}
