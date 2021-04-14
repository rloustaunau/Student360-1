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

@Component({
  selector: 'app-attendance-letters',
  templateUrl: './attendance-letters.component.html',
  styleUrls: ['./attendance-letters.component.css']
})

export class AttendanceLettersComponent implements OnInit, OnDestroy {
  subscription: Subscription;
  filters: GridFilter[];
  gridRequest: GridRequest;
  availableFilterColumns: string[];
  studentUniqueId: string;
  loading = false;
  selectedAll = false;
  currentDate: Date;
  grid: Grid;
  maxDate: string;
  dateFilter: GridFilter;
  statusFilter: GridFilter;
  typeFilter: GridFilter;
  statuses: AttendanceLetterStatus[];
  visible = false;
  @ViewChild('searchInput', null) searchInput: ElementRef;

  constructor(public apiService: ApiService, private route: ActivatedRoute,
    private toastr: ToastrService, private modalService: NgbModal) {
    this.statuses = [];
    var today = new Date();
    today.setHours(0, 0, 0, 0);
    today.setDate(new Date().getDate() - 2);
    this.maxDate = today.toISOString().split('T')[0];
    this.dateFilter = {
      value: undefined, column: "LastAbsence", operator: "<=", type: "select",
      defaultValue: false, formatType: null, maxValue: null, placeholder: "Qualifying Absence", methodName: null,
      serviceName: null, options: []
    };
    this.statusFilter = {
      value: 4, column: "AttendanceLetterStatusId", operator: "==", type: "select",
      defaultValue: false, formatType: null, maxValue: null, placeholder: "Status", methodName: null,
      serviceName: null, options: []
    };
    this.typeFilter = {
      value: undefined, column: "AttendanceLetterTypeId", operator: "==", type: "select",
      defaultValue: false, formatType: null, maxValue: null, placeholder: "Type", methodName: null,
      serviceName: null, options: []
    };
    this.availableFilterColumns = ["SchoolId", "GradeLevel"]; // using the properties to know which filters apply to the grid
    this.filters = this.getApplicableFilters(apiService.filter.getFilters());
    this.subscription = apiService.filter.currentFilters.subscribe(result => {
      this.filters = this.getApplicableFilters(result); 
      if (this.currentDate) {
        this.getData();
      }
    });
    this.studentUniqueId = this.route.snapshot.paramMap.get('id');
    this.grid = new Grid();
    if (this.studentUniqueId)
      this.grid.searchTerm = this.studentUniqueId;
  }

  ngOnInit() {
    this.apiService.letter.GetAttendanceLetterStatus().subscribe(result => {
      this.statuses = result;
    });

    this.apiService.currentUser.getProfile().subscribe(person => {
      this.apiService.currentUser.getAssignmentByStaffUsi(person.usi).subscribe(assignment => {
        var level = assignment.find(element => element.staffClassificationDescriptorId == 40789 || element.staffClassificationDescriptorId == 40791);
        level == undefined ? this.visible = false : this.visible = true; 
      });
    });
    
  }


  dateChanged() {
    if (this.currentDate) {
      this.dateFilter.value = this.currentDate;
      this.getData();
    }
  }

  statusChanged() {
    if (this.currentDate) {
      this.getData();
    }
  }

  typeChanged(type) {
    if (this.typeFilter.value != type.typeId) {
      this.typeFilter.value = type.typeId;
    } else {
      this.typeFilter.value = undefined;
    }

    this.getData();
  }

  confirmArchive() {
    const modalRef = this.modalService.open(MessageResultModalComponent);
    modalRef.componentInstance.title = 'Confirm Action';
    modalRef.componentInstance.successButtonName = 'Continue';
    modalRef.componentInstance.message = `ARE YOU SURE?  Archiving Selected Items will OVERRIDE all Open or Unsent letters that you have selected and they will no longer appear in this view. `;
    modalRef.result.then((result) => {
      if (result) {
        this.archiveLetters();
      }
    });
  }

  confirmPrint() {

    if (this.statusFilter.value == 3) {
      this.printLetters();
    } else {
      const modalRef = this.modalService.open(MessageResultModalComponent);
      modalRef.componentInstance.title = 'Confirm Action';
      modalRef.componentInstance.successButtonName = 'Continue';
      modalRef.componentInstance.message = `ARE YOU SURE?  Printing Selected Letters produces a PDF for printing, and ALSO permanently marks them as 'SENT'.`;
      modalRef.result.then((result) => {
        console.log(result);
        if (result) {
          this.printLetters();
        }
      });
    }

  }

  checkSelected() {
    return !(this.grid && this.grid.data && this.grid.data.filter(x => x.selected).length > 0);
  }

  printLetters() {
    var selectedList = this.grid.data.filter(x => x.selected);

    this.loading = true;
    if (this.statusFilter.value != 4) {
      this.apiService.letter.ReprintAttendanceLetters(selectedList).subscribe(result => {
        this.loading = false;
        if (this.statusFilter.value != 3) {
          this.grid.data = this.grid.data.filter(x => !x.selected);
        }
        saveAs(result.file, result.fileName + '.zip', {
          type: 'application/zip'
        });

        this.toastr.success("The letter has been generated successfully.");
      });
    } else {
      this.apiService.letter.SendAttendanceLetters(selectedList).subscribe(result => {
        this.loading = false;
        this.grid.data = this.grid.data.filter(x => !x.selected);
        saveAs(result.file, result.fileName + '.zip', {
          type: 'application/zip'
        });

        this.toastr.success("The letter has been generated successfully.");
      });
    }

  }

  archiveLetters() {
    var selectedList = this.grid.data.filter(x => x.selected);

    if (selectedList.some(x => x.comments == null || x.comments.length == 0)) {
      this.toastr.error('You must add the comments in order to archive the letters.');
    } else {
      var today = new Date();
      selectedList.forEach(element => {
        element.attendanceLetterStatusId = 5; // Archived
      });

      this.loading = true;
      this.apiService.letter.UpdateAttendanceLetterBulk(selectedList).subscribe(result => {
        this.loading = false;
        this.grid.data = this.grid.data.filter(x => !x.selected);
        this.toastr.success("The letters have been archived.");
      });
    }
  }

  searchChanged(searchTerm: string) {
    this.grid.pageNumber = 1;
    this.grid.searchTerm = searchTerm;
    this.getData();
  }

  updatePageSize(pageSize: number) {
    this.grid.pageSize = +pageSize;
    this.getData();
  }

  changeAll() {
    if (this.selectedAll)
      this.grid.data.forEach(x => {
        x.selected = true;
      });
    else
      this.grid.data.forEach(x => {
        x.selected = false;
      });
  }

  getApplicableFilters(filters: GridFilter[]) {
    if (!filters)
      return [];
    return filters.filter(x => x.value && this.availableFilterColumns.some(key => key.toUpperCase() == x.column.toUpperCase()))
      .concat([this.typeFilter, this.dateFilter, this.statusFilter]);
  }

  getLetterAge(date: Date) {
    date = new Date(date);
    return Math.ceil((Math.abs(date.getTime() - new Date().getTime())) / (1000 * 60 * 60 * 24));
  }

  getData() {
    this.loading = true;
    this.grid.filters = this.getApplicableFilters(this.filters); 
    this.gridRequest = new GridRequest(this.grid);
    this.gridRequest.allData = true;
    this.apiService.letter.GetAttendanceLetterGrid(this.gridRequest).subscribe(result => {
      this.loading = false; 
      this.grid.data = result.data;
      this.grid.filteredCount = result.filteredCount;
      this.grid.maxPageNumber = Math.ceil(this.grid.filteredCount / this.grid.pageSize);
      this.grid.metadata = result.metadata; 
    }, error => {
      this.loading = false;
    });
  }

  pageChanged(page) {
    this.grid.pageNumber = page;
    this.getData();
  }

  updateYear() {
    this.getData();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  processChildUpdate(item, data: StudentExtraHourGrid[]) {
    item.totalHours = data.reduce((pv, cv) => pv + cv.hours, 0);
  }

  openStudentModal(studentUsi: number) {
    const modalRef = this.modalService.open(AttendanceDetailModalComponent, { size: "lg" });
    modalRef.componentInstance.studentUsi = studentUsi;
  }

  openActionsModal(student: any) {
    const modalRef = this.modalService.open(StudentHourCurrentComponent, {
      size: 'xl'
    });
    modalRef.componentInstance.object = student; 
  }
}
