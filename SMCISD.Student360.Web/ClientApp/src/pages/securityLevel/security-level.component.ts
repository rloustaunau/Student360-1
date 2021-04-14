import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Subscription, fromEvent } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { StudentExtraHourGrid } from '@app/services/api/student-extra-hour.service';
import { Grid, GridFilter, GridRequest } from '@app/components/data-grid/data-grid.component';
import { ApiService } from '@app/services/api/api.service';
import { AttendanceLetterStatus } from '@app/services/api/attendance-letter.service';
import { MessageResultModalComponent } from '@app/components/modals/message-result-modal/message-result-modal.component';
import { saveAs } from 'file-saver';
import { AttendanceDetailModalComponent } from '../../app/components/modals/attendance-detail-modal/attendance-detail-modal.component';
import { StudentHourCurrentComponent } from '../../app/components/student-hour-current/student-hour-current.component';
import { People, StaffAccessLevel, StaffEducationOrganizationAssignmentAssociation } from '../../app/services/api/current-user.service';
import { AddStaffAccessLevelModalComponent } from '../../app/components/modals/add-staff-access-level/add-staff-access-level-modal.component';

@Component({
  selector: 'app-security-level',
  templateUrl: './security-level.component.html',
  styleUrls: ['./security-level.component.css']
})

export class SecurityLevelComponent implements OnInit, OnDestroy {
  subscription: Subscription;  
  loading = false;
  peoples: People[];
  staff: StaffEducationOrganizationAssignmentAssociation[];
  schools: any;
  schoolName: string;
  staffAccessLevel: StaffAccessLevel[];
  people: People;
  isDisable: boolean;
  tableSelected: any;

  @ViewChild('searchInput', null) searchInput: ElementRef;

  constructor(public apiService: ApiService, private route: ActivatedRoute,
    private toastr: ToastrService, private modalService: NgbModal) {

    this.peoples = [];
    this.staff = [];
    this.schools = [];
    this.staffAccessLevel = [];
  }

  ngOnInit() {
    this.apiService.school.getSchools().subscribe(result => {
      this.schools = result; 
    });
    this.apiService.currentUser.getStaffAccessLevel().subscribe(result => {
      this.staffAccessLevel = result; 
    });

    this.isDisable = true;
  }

  searchChanged(searchTerm: string) {
    
    this.apiService.currentUser.getStaffByName(searchTerm).subscribe(result => {
      this.peoples = result;
    });
  }

  getAssignmentAssociation(people: People) {
    this.tableSelected = people.usi;
    this.people = people;
    this.apiService.currentUser.getAssignmentByStaffUsi(people.usi).subscribe(result => {
      this.staff = result;
      this.isDisable = false;
    });
  }

  ngOnDestroy(): void {
    //this.subscription.unsubscribe();
  }

  getSchoolName(id: number) {
    var school = this.schools.find(s => s.schoolId == id);
    return school != undefined ? school.nameOfInstitution : "-";
  }

  getStaffTitle(id: string) {
    var title = this.staffAccessLevel.find(s => s.staffClassificationDescriptorId == id);
    return title != undefined ? title.description : "-";
  }
  getStaffLevel(id: string) {
    var level = this.staffAccessLevel.find(s => s.staffClassificationDescriptorId == id);
    return level != undefined ? level.id : "-";
  }

  showAddModal() {
    const modalRef = this.modalService.open(AddStaffAccessLevelModalComponent, { size: "sm" });
    modalRef.componentInstance.staffUSI = this.people.usi;
    modalRef.componentInstance.schools = this.schools;
    modalRef.componentInstance.staffAccessLevel = this.staffAccessLevel;
    modalRef.result.then((result: StaffEducationOrganizationAssignmentAssociation) => {
      if (result) {
        this.getAssignmentAssociation(this.people);
      }
    });
  }

  showEditModal(staff: StaffEducationOrganizationAssignmentAssociation) {
    const modalRef = this.modalService.open(AddStaffAccessLevelModalComponent, { size: "sm" });
    modalRef.componentInstance.staffUSI = this.people.usi;
    modalRef.componentInstance.schools = this.schools;
    modalRef.componentInstance.staffAccessLevel = this.staffAccessLevel;
    modalRef.componentInstance.staffInitial = staff;
    modalRef.result.then((result: StaffEducationOrganizationAssignmentAssociation) => {
      if (result) {
        this.getAssignmentAssociation(this.people);
      }
    });
  }

  confirmDelete(staff: StaffEducationOrganizationAssignmentAssociation) {
    const modalRef = this.modalService.open(MessageResultModalComponent);
    modalRef.componentInstance.title = 'Confirm Action';
    modalRef.componentInstance.successButtonName = 'Continue';
    modalRef.componentInstance.message = `ARE YOU SURE?  Deleting this element. `;
    modalRef.result.then((result) => {
      if (result) {
        this.delete(staff);
      }
    });
  }

  delete(staff: StaffEducationOrganizationAssignmentAssociation) {
    this.apiService.currentUser.deleteAssignmentAssociation(staff).subscribe(result => {
      this.toastr.success('The record was deleted successfully.');
      this.getAssignmentAssociation(this.people);
    });
  }
}
