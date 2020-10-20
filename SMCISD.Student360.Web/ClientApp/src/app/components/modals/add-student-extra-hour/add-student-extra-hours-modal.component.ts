import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component } from '@angular/core';
import { Reason, StudentExtraHours } from '@app/services/api/student-extra-hour.service';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-add-student-extra-hours-modal',
  templateUrl: './add-student-extra-hours-modal.component.html'
})

export class AddStudentExtraHoursModalComponent implements OnInit {
  reasons: Reason[] = [];
  @Input() studentUniqueId: string;
  studentExtraHour: StudentExtraHours;

  constructor(public activeModal: NgbActiveModal, public apiService: ApiService, public toastrService: ToastrService) {
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
      comments: null
    };
  }

  ngOnInit() {
    this.apiService.studentExtraHour.getReasons().subscribe(result => {
      this.reasons = result;
    });
  }

  create() {
    if (!this.studentExtraHour.reasonId || !this.studentExtraHour.comments) {
      this.toastrService.info("Please fill all the fields.")
      return;
    }
    this.studentExtraHour.studentUniqueId = this.studentUniqueId;
    this.studentExtraHour.reasonId = +this.studentExtraHour.reasonId;
    this.apiService.studentExtraHour.createStudentHours(this.studentExtraHour).subscribe(result => {
      this.toastrService.success('The record was created successfully.');
      this.activeModal.close(result);
    });
  }

}
