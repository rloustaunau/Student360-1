import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { ApiService } from '../../services/api/api.service';
import { StudentAbsencesByCourse } from '../../services/api/student.service';
import { Observable, forkJoin, Subscription } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AttendanceModalComponent } from '../modals/attendance-modal/attendance-modal.component';
import { GridRequest, GridFilter } from '../data-grid/data-grid.component';
import { AuthenticationService, Token } from '../../services/authentication/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { Reason, StudentExtraHourGrid } from '../../services/api/student-extra-hour.service';
import { AddStudentExtraHoursModalComponent } from '../modals/add-student-extra-hour/add-student-extra-hours-modal.component';

@Component({
  selector: 'app-student-hour-history',
  templateUrl: './student-hour-history.component.html',
  styleUrls: ['./student-hour-history.component.css']
})

export class StudentHourHistoryComponent implements OnInit, OnDestroy {
  // Uncomment when loading from external controller
  @Input() object: any;
  loading: boolean;
  list: StudentExtraHourGrid[];
  imageUrl = '';
  studentExtraHours = 0;
  request: GridRequest;
  tokenSubscription: Subscription;
  token: Token;
  reasons: Reason[];


  constructor(private apiService: ApiService, private modalService: NgbModal, private auth: AuthenticationService, private toastr: ToastrService) {
    this.list = [];
    this.reasons = [];
    this.request = {
      pageNumber: 1,
      pageSize: 25,
      orderBy: [],
      searchTerm: undefined,
      filters: [],
      select: [],
      allData: true
    };
    this.tokenSubscription = this.auth.tokenInfo.subscribe(result => {
      this.token = result;
    });
  }

  ngOnInit(): void {
    this.token = this.auth.currentTokenValue;
    var filter = { value: this.object['studentUniqueId'], column: "StudentUniqueId", operator: "==", options: [], isBoolean: false, serviceName: null, methodName: null, defaultValue: true, placeholder: null, formatType: null, isRange: false, maxValue: null };
    this.request.filters.push(filter);
    this.loading = true;
    this.apiService.studentExtraHour.GetHistoryHoursById(this.request).subscribe(result => {
      this.loading = false;
      this.list = result.data;
    },
      error => {
        this.loading = false;
      });

    this.apiService.studentExtraHour.getReasons().subscribe(result => {
      this.reasons = result;
    });
  }

  isValid(userId: string): boolean {
    return this.token.person_unique_id == userId || +this.token.level_id < 3;
  }

  canAdd() {
    return +this.token.level_id > 3;
  }
  updateRecord(item) {
    this.apiService.studentExtraHour.updateStudentHours(item).subscribe(result => {
      this.toastr.success("The record has been updated successfully.");
    }, error => {
      this.toastr.error(error.message || error);
    });
  }

  createRecord() {
    const modalRef = this.modalService.open(AddStudentExtraHoursModalComponent);
    modalRef.componentInstance.studentUniqueId = this.object['studentUniqueId'];
    modalRef.result.then((result: StudentExtraHourGrid) => {
      if (result) {
        result.reason = this.reasons.find(x => x.reasonId == result.reasonId).value;
        result.userFirstName = this.token.firstname;
        result.userLastSurname = this.token.lastsurname;
        this.list.push(result);
      }
    })
  }

  ngOnDestroy() {
    this.tokenSubscription.unsubscribe();
  }
}
