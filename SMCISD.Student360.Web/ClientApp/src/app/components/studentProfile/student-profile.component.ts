import { Component, Input, OnInit } from '@angular/core';
import { ApiService } from '../../services/api/api.service';
import { StudentAbsencesByCourse } from '../../services/api/student.service';
import { Observable, forkJoin } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AttendanceModalComponent } from '../modals/attendance-modal/attendance-modal.component';
import { GridRequest, GridFilter, Grid } from '../data-grid/data-grid.component';
import { Router } from '@angular/router';
import { AuthenticationService, Token } from '../../services/authentication';
import { AttendanceDetailModalComponent } from '../modals/attendance-detail-modal/attendance-detail-modal.component';

@Component({
  selector: 'app-student-profile',
  templateUrl: './student-profile.component.html',
  styleUrls: ['./student-profile.component.css']
})

export class StudentProfileComponent implements OnInit {
  // Uncomment when loading from external controller
  @Input() object: any;
  loading: boolean;
  isHighSchoolStudent: boolean = false;
  isMiddleSchoolStudent: boolean = false;
  grid: Grid;
  imageUrl: string = "";
  studentExtraHours = 0;
  request: GridRequest;
  userIsAdmin: boolean;
  token: Token;

  constructor(private apiService: ApiService, private modalService: NgbModal, private router: Router, private auth: AuthenticationService) {
    this.grid = new Grid();
    this.request = {
      pageNumber: 1,
      pageSize: 25,
      orderBy: [],
      searchTerm: undefined,
      filters: [],
      select: [],
      allData: true
    };
  }

  getData(): Observable<any> {
    var filter = { value: this.object['studentUsi'], column: "StudentUsi", operator: "==", options: [], isBoolean: false, serviceName: null, methodName: null, defaultValue: true, placeholder: null, formatType: null, isRange: false, maxValue: null };
    var filter2 = { value: this.object['studentUniqueId'], column: "StudentUniqueId", operator: "==", options: [], isBoolean: false, serviceName: null, methodName: null, defaultValue: true, placeholder: null, formatType: null, isRange: false, maxValue: null };
    this.request.filters.push(filter);
    const response2 = this.apiService.studentExtraHour.getStudentExtraHours(this.request);
    this.request.filters.push(filter2);
    const response1 = this.apiService.student.getStudentProfile(this.request);
    return forkJoin([response1, response2]);
  }

  ngOnInit(): void {
    this.token = this.auth.currentTokenValue;
    this.userIsAdmin = +this.token.level_id == 0;
    this.loading = true;
    this.isHighSchoolStudent = <number>this.object['gradeLevel'] >= 9;
    this.isMiddleSchoolStudent = <number>this.object['gradeLevel'] < 9 && <number>this.object['gradeLevel'] >= 5;

    this.getData().subscribe(result => {
      this.loading = false;
      this.grid = result[0].grid;
      this.imageUrl = result[0].imageUrl;
      this.studentExtraHours = result[1].data.reduce((a, b) => a + (b.hours || 0), 0);
    },
      error => {
        this.loading = false;
      });
  }

  openStudentModal(studentUsi: number) {
    const modalRef = this.modalService.open(AttendanceDetailModalComponent);
    modalRef.componentInstance.studentUsi = studentUsi;
  }

  openAttendanceSummaryModal(studentUsi: number) {
    const modalRef = this.modalService.open(AttendanceModalComponent);
    modalRef.componentInstance.studentUsi = studentUsi;
  }

  goToStudentExtraHoursCrud() {
    this.router.navigate(['/hours/' + this.object.studentUniqueId]);
  }
}
