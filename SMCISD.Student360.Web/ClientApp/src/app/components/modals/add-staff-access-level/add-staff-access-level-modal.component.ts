import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component } from '@angular/core';
import { Reason, StudentExtraHourGrid, StudentExtraHours } from '@app/services/api/student-extra-hour.service';
import { ToastrService } from 'ngx-toastr';
import { MessageResultModalComponent } from '../message-result-modal/message-result-modal.component';
import { StaffAccessLevel, StaffEducationOrganizationAssignmentAssociation } from '../../../services/api/current-user.service';


@Component({
  selector: 'app-add-staff-access-level-modal',
  templateUrl: './add-staff-access-level-modal.component.html'
})

export class AddStaffAccessLevelModalComponent implements OnInit {
  //reasons: Reason[] = [];
  @Input() staffUSI: number;
  @Input() id: string;
  @Input() staffAccessLevel: StaffAccessLevel[];
  @Input() schools: any;
  @Input() staffInitial: StaffEducationOrganizationAssignmentAssociation;
  staff: StaffEducationOrganizationAssignmentAssociation;

  constructor(public activeModal: NgbActiveModal, public apiService: ApiService, public toastrService: ToastrService, private modalService: NgbModal) {
    this.staff = {
      id: this.id,
      beginDate:new Date,
      educationOrganizationId: 0,
      staffClassificationDescriptorId: 0,
      staffUSI: this.staffUSI,
      positionTitle: null,
      endDate: null,
      orderOfAssignment: null,
      employmentEducationOrganizationId: null,
      employmentStatusDescriptorId: null,
      employmentHireDate: null,
      credentialIdentifier: null,
      stateOfIssueStateAbbreviationDescriptorId: null,
      discrimintor: null,
      createDate:new Date,
      lastModifiedDate:new Date,
    };
  }

  ngOnInit() {
    if (this.staffInitial != undefined) {
      this.staff.educationOrganizationId = this.staffInitial.educationOrganizationId;
      this.staff.staffClassificationDescriptorId = this.staffInitial.staffClassificationDescriptorId;
      this.staff.id = this.staffInitial.id;
    }
  }

  create() {
    this.staff.staffUSI = this.staffUSI;
    this.staff.educationOrganizationId = parseInt(this.staff.educationOrganizationId.toString());
    this.staff.staffClassificationDescriptorId = parseInt(this.staff.staffClassificationDescriptorId.toString());
    if (!this.staff.educationOrganizationId || !this.staff.staffClassificationDescriptorId) {
      this.toastrService.info("Please fill all the fields.")
      return;
    }

    this.persist();

  }

  persist() {
    
    this.apiService.currentUser.createAssignmentAssociation(this.staff).subscribe(result => {
      this.toastrService.success('The record was saved successfully.');
      this.activeModal.close(result);
    });
  }

 

}
