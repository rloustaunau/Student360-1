import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod, StudentAtRisk } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component, ElementRef, ViewChild } from '@angular/core';
import { GridFilter, Grid, GridRequest } from '../../data-grid/data-grid.component';
import { filter, distinctUntilChanged, tap, debounceTime } from 'rxjs/operators';
import { fromEvent } from 'rxjs';


@Component({
  selector: 'app-student-risk-modal',
  templateUrl: './student-risk-modal.component.html'
})

export class StudentAtRiskModalComponent implements OnInit {
  @Input() studentUsi: number;
  object: StudentAtRisk;
  loading: boolean = false;
  
  @ViewChild('searchInput', null) searchInput: ElementRef;
  constructor(public activeModal: NgbActiveModal, private api: ApiService) {
  }

  ngOnInit() {
    this.getData();
  }

 

  getData() {
    this.loading = true;
    this.object = new StudentAtRisk();
    
    this.api.student.getStudentAtRisk(this.studentUsi).subscribe(result => {
      this.loading = false;
      this.object = result; 
    }, error => {
      this.loading = false;
    });
  }

 
}
