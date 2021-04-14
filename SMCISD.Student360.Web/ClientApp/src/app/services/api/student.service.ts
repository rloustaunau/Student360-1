import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { GridRequest, Grid } from '../../components/data-grid/data-grid.component';

@Injectable({ providedIn: 'root' })
export class StudentApiService {
    controllerName = 'student';

    constructor(private http: HttpClient) { }

  public getStudentHighestAbsenceCourseCount(request: GridRequest) {
    return this.http.post(`${environment.apiUrl}/api/${this.controllerName}/grid`, request);
    }

  public getStudentProfile(request: GridRequest): Observable<StudentProfile> {
    return this.http.post(`${environment.apiUrl}/api/${this.controllerName}/profile`, request)
          .pipe(map((result: StudentProfile) => result));
    }

  public getDnaChartData(request: GridRequest) {
    return this.http.post(`${environment.apiUrl}/api/${this.controllerName}/dna`, request);
    }

  public getStudentAbsencesCodesByPeriod(studentUsi: number): Observable<GeneralStudentDnaData> {
    return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/absencescodes/${studentUsi}`)
        .pipe(map((result: any) => result));
  }

  public getStudentAttendanceDetail(request: GridRequest): Observable<Grid> {
    return this.http.post(`${environment.apiUrl}/api/${this.controllerName}/attendance`, request)
      .pipe(map((result: Grid) => result));
  }

  public getStudentAtRisk(studentUsi: number): Observable<StudentAtRisk> {
    return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/atRisk/${studentUsi}`)
      .pipe(map((result: any) => result));
  }

  public getStudentCourseTranscript(request: GridRequest): Observable<Grid> {
    return this.http.post(`${environment.apiUrl}/api/${this.controllerName}/courseTranscript`, request)
      .pipe(map((result: Grid) => result));
  }
}

export class StudentAbsencesByCourse {
    studentUsi: number;
    studentUniqueId: string;
    studentFirstName: string;
    schoolId: number;
    studentLastSurname: string;
    gradeLevel: string;
    graduationSchoolYear : number;
    schoolYear: number;
    nameOfInstitution: string;
    sessionName: string;
    localCourseCode: string;
    classPeriodName: string;
    localCourseTitle: string;
    teacherLastSurname: string;
    classroomPositionDescriptorCodeValue: string;
    absencesCount : number;
    sectionIdentifier: string;
    mark9w1: string;
    mark9w2: string;
    mark9w3: string;
    mark9w4: string;
    fs1: string;
    fs2: string;
    yfinal: string;
}

export class StudentAbsencesCodesByPeriod {
  studentUsi: number;
  classPeriodName: string;
  absenceCodes: AbsencesCodesByPeriod[];
}
export class StudentAtRisk {
  studentUsi: number;
  isHomeless: boolean;
  section504: boolean;
  ar: boolean;
  ssi: boolean;
  ell: boolean;
  prePregnant: boolean;
  preParent: boolean;
  aep: boolean;
  expelled: boolean;
  dropout: boolean;
  lep: boolean;
  fosterCare: boolean;
  residentialPlacementFacility: boolean;
  incarcerated: boolean;
  adultEd: boolean;
  prs: boolean;
  notAdvanced: boolean;
}
export class GeneralStudentDnaData {
  periods: StudentAbsencesCodesByPeriod[];
  gpa: number;
  nameOfInstitution: string;
  streetNumberName: string;
  apartmentRoomSuiteNumber: string;
  city: string;
  state: string;
  postalCode: string;
}

export class AbsencesCodesByPeriod {
  absenceCode: string;
  quantity: number;
  description: string;
}

export class StudentProfile {
  courses: StudentAbsencesByCourse[];
  imageUrl: string;
}
