import { Component, Input, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { ApiService } from '../../services/api/api.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridRequest } from '../data-grid/data-grid.component';
import { AuthenticationService, Token } from '../../services/authentication/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { Reason, StudentExtraHourGrid } from '../../services/api/student-extra-hour.service';
import { AddStudentExtraHoursModalComponent } from '../modals/add-student-extra-hour/add-student-extra-hours-modal.component';
import { StudentExtraHourHistoryComponent } from '../modals/student-extra-hour-history/student-extra-hour-history.component';
import { Subscription } from 'rxjs';
import { MessageResultModalComponent } from '../modals/message-result-modal/message-result-modal.component';
import { downloadCsv, generateCsv } from '../shared/file.helper';

@Component({
  selector: 'app-student-hour-current',
  templateUrl: './student-hour-current.component.html',
  styleUrls: ['./student-hour-current.component.css']
})

export class StudentHourCurrentComponent implements OnInit, OnDestroy {
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
  reasonsCopy: Reason[];
  isHighSchoolStudent: boolean = false;
  isMiddleSchoolStudent: boolean = false;
  isElementarySchoolStudent: boolean = false;
  @Output()
  updateSum = new EventEmitter<StudentExtraHourGrid[]>();

  constructor(private apiService: ApiService, private modalService: NgbModal, private auth: AuthenticationService, private toastr: ToastrService) {
    this.list = [];
    this.reasons = [];
    this.reasonsCopy = [];
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
    this.isHighSchoolStudent = this.object['gradeDescription'] == "Hight";//<number>this.object['gradeLevel'] >= 9;
    this.isMiddleSchoolStudent = this.object['gradeDescription'] == "Middle"; //<number>this.object['gradeLevel'] < 9 && <number>this.object['gradeLevel'] >= 5;
    this.isElementarySchoolStudent = this.object['gradeDescription'] == "Elementary"; //!this.isHighSchoolStudent && !this.isMiddleSchoolStudent;
    this.token = this.auth.currentTokenValue;
    var filter = { value: this.object['studentUniqueId'], column: "StudentUniqueId", operator: "==", options: [], isBoolean: false, serviceName: null, methodName: null, defaultValue: true, placeholder: null, formatType: null, isRange: false, maxValue: null };
    var yearFilter = { value: this.object['schoolYear'], column: "SchoolYear", operator: "==", options: [], isBoolean: false, serviceName: null, methodName: null, defaultValue: true, placeholder: null, formatType: null, isRange: false, maxValue: null };
    this.request.filters.push(filter);
    this.request.filters.push(yearFilter);
    this.loading = true;
    this.apiService.studentExtraHour.GetCurrentStudentExtraHours(this.request).subscribe(result => {
      this.loading = false; 
      this.list = result.data;
    },
      error => {
        this.loading = false;
      });

    this.apiService.studentExtraHour.getReasons().subscribe(result => {
      this.reasons = result;
      this.reasonsCopy = this.reasons.slice(3);
    });
  }

  openStudentExtraHourHistory(item: StudentExtraHourGrid) {
    const modalRef = this.modalService.open(StudentExtraHourHistoryComponent);
    modalRef.componentInstance.studentUsi = this.object['studentUsi'];
    modalRef.componentInstance.currentRecord = item;
  }

  isValid(userId: string): boolean {
    return this.token.person_unique_id == userId || +this.token.level_id < 3;
  }


  isValidList() {
    return (this.list.every(x => x.userCreatedUniqueId == this.token.person_unique_id) || +this.token.level_id < 3)
      && this.list.some(x => x.edited);
  }

  canAdd() {
    return +this.token.level_id > 3;
  }

  export() {

    this.list.forEach(function (element) {
      var createdDate = new Date(element.createDate); 
      element.userRole = createdDate.getDate() + "/" + (createdDate.getMonth() + 1) + "/" + createdDate.getFullYear(); 
    });

    downloadCsv(generateCsv(this.list, ['date', 'reason', 'comments', 'hours', 'userFirstName', 'userLastSurname', 'userRole'],
      ['Date', 'Reason', 'Comments', 'Hours', 'User FirstName', 'User LastSurname', 'Created Date'],
      ['Student Name:', this.object.lastSurname, this.object.firstName, 'StudentID:', this.object.studentUniqueId]),
      `StudentAttendanceActions-${this.object.studentUniqueId}-${new Date().toDateString().split('T')[0]}`);
  }

  updateRecords() {
    var editedRecords = this.list.filter(x => x.edited);

    if (editedRecords.some(r => {
      var reason = this.reasons.find(rea => rea.reasonId == r.reasonId);
      return !reason.hasHours && r.hours > 0;
    })) {
      this.cancelUpdateAttendanceAction();
    } else if (editedRecords.some(r => {
      var reason = this.reasons.find(rea => rea.reasonId == r.reasonId);
      return reason.hasHours && r.hours == 0;
    })) {
      this.confirmUpdateAttendanceAction(editedRecords);
    } else {
      this.persistRecords(editedRecords);
    }
  }

  persistRecords(editedRecords) {
    this.loading = true;
    editedRecords.forEach(item => {
      item.reasonId = +item.reasonId;
    });
    this.apiService.studentExtraHour.updatebulkStudentHours(editedRecords).subscribe(result => {
      this.toastr.success("The record has been updated successfully.");
      this.loading = false;
      this.updateSum.emit(this.list);
      this.list.forEach(item => {
        if (item.edited) {
          item.edited = false;
          item.version++;
        }
      });
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
    });
  }

  confirmUpdateAttendanceAction(editedRecords) {
    const modalRef = this.modalService.open(MessageResultModalComponent);
    modalRef.componentInstance.title = 'No Makeup Hours are assigned';
    modalRef.componentInstance.successButtonName = 'Save Anyway';
    modalRef.componentInstance.message = `You are saving an Action with a 'Reason' for SECONDARY Makeup Hours, and you have 0 (Zero) makeup hours assigned.`;
    modalRef.result.then((result: StudentExtraHourGrid) => {
      if (result) {
        this.persistRecords(editedRecords);
      }
    });
  }

  cancelUpdateAttendanceAction() {
    const modalRef = this.modalService.open(MessageResultModalComponent);
    modalRef.componentInstance.title = 'Invalid Reason Selected';
    modalRef.componentInstance.successButtonName = 'Save Anyway';
    modalRef.componentInstance.showSucessButton = false;
    modalRef.componentInstance.message = `The'Reason' you selected does not allow Makeup Hours. You must first save the Action with (Zero) 0 hours then try to edit again.`;
  }

  ngOnDestroy() {
    this.tokenSubscription.unsubscribe();
  }
}
