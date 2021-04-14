
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Grid, GridRequest } from '../../components/data-grid/data-grid.component';

@Injectable({ providedIn: 'root' })
export class StudentExtraHourService {
  controllerName = 'studentExtraHour';

  constructor(private http: HttpClient) { }

  public importStudentExtraHours(studentExtraHours: StudentExtraHours[]): Observable<StudentExtraHours[]> {
    return this.http.post<StudentExtraHours[]>(`${environment.apiUrl}/api/${this.controllerName}/import/`, studentExtraHours);
  }

  public getStudentExtraHourGrid(request: GridRequest): Observable<Grid> {
    return this.http.post<Grid>(`${environment.apiUrl}/api/${this.controllerName}/grid`, request);
  }

  public GetCurrentStudentExtraHours(request: GridRequest): Observable<Grid> {
    return this.http.post<Grid>(`${environment.apiUrl}/api/${this.controllerName}/current`, request);
  }

  public GetHistoryHoursById(request: GridRequest): Observable<Grid> {
    return this.http.post<Grid>(`${environment.apiUrl}/api/${this.controllerName}/history`, request);
  }

  public createStudentHours(request: StudentExtraHours): Observable<StudentExtraHours> {
    return this.http.post<StudentExtraHours>(`${environment.apiUrl}/api/${this.controllerName}/create`, request);
  }

  public updateStudentHours(request: StudentExtraHours): Observable<StudentExtraHours> {
    return this.http.put<StudentExtraHours>(`${environment.apiUrl}/api/${this.controllerName}`, request);
  }
  public updatebulkStudentHours(request: StudentExtraHours[]): Observable<StudentExtraHours[]> {
    return this.http.put<StudentExtraHours[]>(`${environment.apiUrl}/api/${this.controllerName}/bulk`, request);
  }

  public getReasons(): Observable<Reason[]> {
    return this.http.get<Reason[]>(`${environment.apiUrl}/api/${this.controllerName}/reasons`);
  }
}


export class StudentExtraHourGrid {
  studentExtraHoursId: number;
  version: number;
  studentUniqueId: string;
  gradeLevel: string;
  firstName: string;
  lastSurname: string;
  date: Date;
  hours: number;
  userCreatedUniqueId: string;
  userRole: string;
  createDate: Date;
  studentUsi: number;
  schoolId: number;
  localEducationAgencyId: number;
  schoolYear: number;
  userName: string;
  userFirstName: string;
  userLastSurname: string;
  reason: string;
  reasonId: number;
  comments: string;
  id: string;
  edited: boolean;
}

export class StudentExtraHours {
  studentExtraHoursId: number;
  version: number;
  studentUniqueId: string;
  gradeLevel: string;
  firstName: string;
  lastSurname: string;
  date: Date;
  hours: number;
  userCreatedUniqueId: string;
  userRole: string;
  createdDate: Date;
  schoolYear: number;
  userName: string;
  userFirstName: string;
  userLastSurname: string;
  reason: Reason;
  reasonId: number;
  comments: string;
  id: string;
}

export class Reason {
  reasonId: number;
  value: string;
  descriptrion: string;
  hasHours: boolean;
  createdDate: Date;
}
