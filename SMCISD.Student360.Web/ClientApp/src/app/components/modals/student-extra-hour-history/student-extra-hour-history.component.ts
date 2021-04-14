import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component, ElementRef, ViewChild } from '@angular/core';
import { GridFilter, Grid, GridRequest } from '../../data-grid/data-grid.component';
import { StudentExtraHourGrid } from '@app/services/api/student-extra-hour.service';


@Component({
  selector: 'app-student-extra-hour-history',
  templateUrl: './student-extra-hour-history.component.html'
})

export class StudentExtraHourHistoryComponent implements OnInit {
  @Input() studentUsi: number;
  @Input() currentRecord: StudentExtraHourGrid;
  filters: GridFilter[];
  loading = false;
  grid: Grid;
  constructor(public activeModal: NgbActiveModal, private api: ApiService) {
  }

  ngOnInit() {
    /*this.filters = [
      {
        value: this.studentUsi, column: "StudentUsi", operator: "==", type: "select", defaultValue: true,
        formatType: null, maxValue: null, placeholder: null, methodName: null, serviceName: null, options: []
      },
      {
        value: this.currentRecord.reasonId, column: "ReasonId", operator: "==", type: "select", defaultValue: true,
        formatType: null, maxValue: null, placeholder: null, methodName: null, serviceName: null, options: []
      },
      {
        value: this.currentRecord.date, column: "Date", operator: "==", type: "select", defaultValue: true,
        formatType: null, maxValue: null, placeholder: null, methodName: null, serviceName: null, options: []
      },
      {
        value: this.currentRecord.schoolYear, column: "SchoolYear", operator: "==", type: "select", defaultValue: true,
        formatType: null, maxValue: null, placeholder: null, methodName: null, serviceName: null, options: []
      }];*/
    this.filters = [
      {
        value: this.currentRecord.studentExtraHoursId, column: "StudentExtraHoursId", operator: "==", type: "select", defaultValue: true,
        formatType: null, maxValue: null, placeholder: null, methodName: null, serviceName: null, options: []
      }];

    this.getData();
    this.grid = new Grid();
  }

  getData() {
    this.loading = true;
    var request = new GridRequest(new Grid());
    //this.filters[2].value = this.filters[2].value.split('T')[0];
    request.filters = this.filters; 
    request.allData = true;
    request.orderBy = [{ column: 'Date', direction: 'desc' }, { column: 'CreateDate', direction: 'desc' }]
    this.api.studentExtraHour.GetHistoryHoursById(request).subscribe(result => {
      this.loading = false;
      this.grid = result; 
    }, error => {
      this.loading = false;
    });
  }
}
