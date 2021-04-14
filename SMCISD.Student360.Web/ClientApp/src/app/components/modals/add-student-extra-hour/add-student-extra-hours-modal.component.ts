import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component } from '@angular/core';
import { Reason, StudentExtraHourGrid, StudentExtraHours } from '@app/services/api/student-extra-hour.service';
import { ToastrService } from 'ngx-toastr';
import { MessageResultModalComponent } from '../message-result-modal/message-result-modal.component';


@Component({
  selector: 'app-add-student-extra-hours-modal',
  templateUrl: './add-student-extra-hours-modal.component.html'
})

export class AddStudentExtraHoursModalComponent implements OnInit {
  reasons: Reason[] = [];
  @Input() studentUniqueId: string;
  @Input() isHighSchoolStudent;
  @Input() isMiddleSchoolStudent = false;
  @Input() isElementarySchoolStudent = false;
  studentExtraHour: StudentExtraHours;

  constructor(public activeModal: NgbActiveModal, public apiService: ApiService, public toastrService: ToastrService, private modalService: NgbModal) {
    this.studentExtraHour = {
      studentUniqueId: this.studentUniqueId,
      gradeLevel: null,
      schoolYear: 0,
      firstName: null,
      lastSurname: null,
      date: new Date(),
      hours: 0,
      userCreatedUniqueId: null,
      id: undefined,
      userRole: null,
      createdDate: new Date(),
      userFirstName: null,
      userLastSurname: null,
      userName: null,
      reasonId: undefined,
      reason: null,
      comments: null,
      version: 0,
      studentExtraHoursId: 0
    };
  }

  ngOnInit() {
    this.apiService.studentExtraHour.getReasons().subscribe(result => {
      if (this.isElementarySchoolStudent)
        this.reasons = result.filter(reason => !reason.hasHours);
      else
        this.reasons = result;

      this.reasons = this.reasons.slice(3); 
    });

    
  }

  create() {
    if (!this.studentExtraHour.reasonId || !this.studentExtraHour.comments) {
      this.toastrService.info("Please fill all the fields.")
      return;
    }

    var currentReason = this.reasons.find(rea => rea.reasonId == this.studentExtraHour.reasonId);
    if (!currentReason.hasHours && this.studentExtraHour.hours > 0) {
      this.cancelUpdateAttendanceAction();
    } else if (currentReason.hasHours && this.studentExtraHour.hours == 0) {
      this.confirmUpdateAttendanceAction();
    } else {
      this.persist();
    }

  }

  persist() {
    this.studentExtraHour.studentUniqueId = this.studentUniqueId;
    this.studentExtraHour.reasonId = +this.studentExtraHour.reasonId;
    this.apiService.studentExtraHour.createStudentHours(this.studentExtraHour).subscribe(result => {
      this.toastrService.success('The record was created successfully.');
      this.activeModal.close(result);
    });
  }


  confirmUpdateAttendanceAction() {
    const modalRef = this.modalService.open(MessageResultModalComponent);
    modalRef.componentInstance.title = 'No Makeup Hours are assigned';
    modalRef.componentInstance.successButtonName = 'Save Anyway';
    modalRef.componentInstance.message = `You are saving an Action with a 'Reason' for SECONDARY Makeup Hours, and you have 0 (Zero) makeup hours assigned.`;
    modalRef.result.then((result: StudentExtraHourGrid) => {
      if (result) {
        this.persist();
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

}
