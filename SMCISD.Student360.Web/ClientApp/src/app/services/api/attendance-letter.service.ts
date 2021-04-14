
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Grid, GridRequest } from '../../components/data-grid/data-grid.component';


@Injectable({ providedIn: 'root' })
export class AttendanceLetterService {
  controllerName = 'attendanceLetter';

  constructor(private http: HttpClient) { }

  public GetAttendanceLetterGrid(request: GridRequest): Observable<Grid> {
    return this.http.post<Grid>(`${environment.apiUrl}/api/${this.controllerName}`, request);
  }

  public GetAttendanceLetterStatus(): Observable<AttendanceLetterStatus[]> {
    return this.http.get<AttendanceLetterStatus[]>(`${environment.apiUrl}/api/${this.controllerName}/status`);
  }

  public SendAttendanceLetters(letters: any[]) {
    return this.http.put(`${environment.apiUrl}/api/${this.controllerName}/send`,
      letters, { responseType: 'blob', observe: 'response' }).pipe(map(res => {
        var filename = res.headers.getAll('content-disposition')[0].split('=')[1].split(';')[0];;
        return { file: res.body, fileName: filename };
      }));
  }

  public ReprintAttendanceLetters(letters: any[]) {
    return this.http.put(`${environment.apiUrl}/api/${this.controllerName}/reprint`,
      letters, { responseType: 'blob', observe: 'response' }).pipe(map(res => {
        var filename = res.headers.getAll('content-disposition')[0].split('=')[1].split(';')[0];;
        return { file: res.body, fileName: filename };
      }));
  }

  public UpdateAttendanceLetterBulk(letters: any[]): Observable<AttendanceLetterGrid[]> {
    return this.http.put<AttendanceLetterGrid[]>(`${environment.apiUrl}/api/${this.controllerName}/bulk`, letters);
  }
}




export class AttendanceLetterGrid {
  attendanceLetterId: number;
  attendanceLetterTypeId: number;
  attendanceLetterStatusId: number;
  classPeriodName: string;
  firstAbsence: Date;
  lastAbsence: Date;
  resolutionDate: Date;
  studentUniqueId: string;
  firstName: string;
  middleName: string;
  lastSurname: string;
  gradeLevel: string;
  schoolYear: number;
  schoolId: number;
  userCreatedUniqueId: string;
  userFirstName: string;
  userLastSurname: string;
  createDate: Date;
  id: number;
  status: string;
  type: string;
  localEducationAgencyId: number;
  studentUsi: number;
  comments: string;
}

export class AttendanceLetterStatus {
  attendanceLetterStatusId: number;
  codeValue: string;
  shortDescription: string;
  description: string;
}

