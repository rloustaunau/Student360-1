import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

@Injectable({ providedIn: 'root' })
export class CurrentUserApiService {
    controllerName = 'currentUser';

    constructor(private http: HttpClient) { }

    public getProfile() {
        return this.http.get<People>(`${environment.apiUrl}/api/${this.controllerName}/profile`);
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
