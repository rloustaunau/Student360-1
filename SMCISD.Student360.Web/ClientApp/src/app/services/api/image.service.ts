
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

@Injectable({ providedIn: 'root' })
export class ImageApiService {
  controllerName = 'image';

  constructor(private http: HttpClient) { }

  public getStudentImage(studentUniqueId: string) {
    return this.http.get(`${environment.apiUrl}/api/${this.controllerName}/student/${studentUniqueId}`);
  }
}
