
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ReportApiService {
    controllerName = 'report';

    constructor(private http: HttpClient) { }

    public getAdaRates() {
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/adarate`)
        .pipe(map((result: DistrictDailyAttendanceRate[]) => result));
    }

    public getSsrsReports(){
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/reports`)
        .pipe(map((result: Report[]) => result));
  }

  public getYtdRates() {
    return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/ytdrate`)
      .pipe(map((result: YtdRegionRate[]) => result));
  }

  public getYtdSchoolLevels() {
    return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/ytdschoollevel`)
      .pipe(map((result: YtdRegionRate[]) => result));
  }

  public getYtdGradeLevels() {
    return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/ytdgradelevel`)
      .pipe(map((result: YtdRegionRate[]) => result));
  }
}

export class DistrictDailyAttendanceRate {
  membership: string;
  present: string;
  date: Date;
}

export class Report {
  id: number;
  reportName: string;
  reportUri: string;
  levelId: number
}

export class YtdRegionRate {
  nameOfInstitution: string; // Can be "District"
  studentAttendance: number;
  maxStudentAttendance: number;
  attendancePercent: number;
  schoolLevel: string;
  gradeLevel: string;
  schoolYear: number;
}

