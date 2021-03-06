
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

@Injectable({ providedIn: 'root' })
export class SchoolApiService {
    controllerName = 'school';

    constructor(private http: HttpClient) { }

    public getSchools() {
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}`);
    }

    public getAbsenceCountList() {
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/absenceCountList`);
    }

    public getAbsencePercentList() {
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/absencePercentList`);
    }

    public getSchoolYears() {
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/years`);
    }

    public getGrades() {
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/grades`);
    }

    public getCohorts() {
      return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/cohorts`);
    }
}
