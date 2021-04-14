import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CurrentUserApiService {
    controllerName = 'currentUser';

    constructor(private http: HttpClient) { }

    public getProfile() {
        return this.http.get<People>(`${environment.apiUrl}/api/${this.controllerName}/profile`);
    }
    public getStaffAccessLevel(): Observable<StaffAccessLevel[]>  {
        return this.http.get<StaffAccessLevel[]>(`${environment.apiUrl}/api/${this.controllerName}/getStaffAccessLevel`);
    }
    public createAssignmentAssociation(request: StaffEducationOrganizationAssignmentAssociation): Observable<StaffEducationOrganizationAssignmentAssociation> {
      return this.http.post<StaffEducationOrganizationAssignmentAssociation>(`${environment.apiUrl}/api/${this.controllerName}/createAssignmentAssociation`, request);
    }
    public deleteAssignmentAssociation(request: StaffEducationOrganizationAssignmentAssociation): Observable<StaffEducationOrganizationAssignmentAssociation> {
      return this.http.post<StaffEducationOrganizationAssignmentAssociation>(`${environment.apiUrl}/api/${this.controllerName}/deleteAssignmentAssociation`, request);
    }
    public getStaffByName(name: string): Observable<People[]> {
      return this.http.get<People[]>(`${environment.apiUrl}/api/${this.controllerName}/getStaffByName/${name}`);
    }
    public getAssignmentByStaffUsi(usi: number): Observable<StaffEducationOrganizationAssignmentAssociation[]> {
      return this.http.get<StaffEducationOrganizationAssignmentAssociation[]>(`${environment.apiUrl}/api/${this.controllerName}/getAssignmentByStaffUsi/${usi}`);
    }
    public createAccess(request: AccessToSystem): Observable<AccessToSystem> {
      return this.http.post<AccessToSystem>(`${environment.apiUrl}/api/${this.controllerName}/createAccess`, request);
    }
    public getAccessLevel() {
      return this.http.get<AccessLevelDefinition>(`${environment.apiUrl}/api/${this.controllerName}/accessLevel`);
    }
}

export class People
{
    usi: number;
    uniqueId: string;
    firstName: string;
    lastSurname: string;
    electronicMailAddress: string;
    personType: string;
    positionTitle: string;

    edOrgAssociations: EdOrgAssociation[];
    schoolId: number; 
    localEducationAgencyId: number;
}

export class EdOrgAssociation
{
    educationOrganizationId: number;
    edOrgType: string;
    personType: string;
    positionTitle: string;
}
@Injectable({ providedIn: 'root' })
export class AccessToSystem {
  id: number;
  email: string;
  fullName: string;
  lastLogin: Date;
  schoolCode: string;
}

@Injectable({ providedIn: 'root' })
export class AccessLevelDefinition {
  id: number;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class StaffAccessLevel {
  id: string;
  description: string;
  isAdmin: boolean;
  staffClassificationDescriptorId: string;
}


@Injectable({ providedIn: 'root' })
export class StaffEducationOrganizationAssignmentAssociation {
  id: string;
  beginDate: Date;
  educationOrganizationId: number;
  staffClassificationDescriptorId: number;
  staffUSI: number;
  positionTitle: string;
  endDate: Date;
  orderOfAssignment: number;
  employmentEducationOrganizationId: number;
  employmentStatusDescriptorId: number;
  employmentHireDate: Date;
  credentialIdentifier: string;
  stateOfIssueStateAbbreviationDescriptorId: number;
  discrimintor: string;
  createDate: Date;
  lastModifiedDate: Date;
}
