import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component, ElementRef, ViewChild } from '@angular/core';
import { GridFilter, Grid, GridRequest } from '../../data-grid/data-grid.component';
import { filter, distinctUntilChanged, tap, debounceTime } from 'rxjs/operators';
import { fromEvent } from 'rxjs';


@Component({
  selector: 'app-attendance-detail-modal',
  templateUrl: './attendance-detail-modal.component.html'
})

export class AttendanceDetailModalComponent implements OnInit {
  @Input() studentUsi: number;
  filters: GridFilter[];
  loading: boolean = false;
  searchTerm = "";
  grid: Grid;
  @ViewChild('searchInput', null) searchInput: ElementRef;
  constructor(public activeModal: NgbActiveModal, private api: ApiService) {
  }

  ngOnInit() {
    this.filters = [
      { value: this.studentUsi, column: "StudentUsi", operator: "==", type: "select", defaultValue: true, formatType: null, maxValue: null, placeholder: null, methodName: null, serviceName: null, options: [] },
      { value: "Y", column: "State", operator: "==", type: "select", defaultValue: true, formatType: null, maxValue: null, placeholder: "State Code", methodName: null, serviceName: null, options: [{ id: "Y", value: "Yes" }, { id: "N", value: "No" }] }];
    this.getData();
    this.grid = new Grid();
  }

  updateFilter(event: any) {
    let newFilters = this.filters.slice();
    
    newFilters[1] = event;
    this.filters = newFilters;
    this.getData();
  }


  ngAfterViewInit(): void {
      fromEvent(this.searchInput.nativeElement, 'keyup')
        .pipe(
          filter(Boolean),
          debounceTime(1500),
          distinctUntilChanged(),
          tap((text) => {
            this.search(this.searchInput.nativeElement.value);
          })
        ).subscribe();
  }

  search(searchTerm: string) {
    this.searchTerm = searchTerm;
    this.getData();
  }

  getData() {
    this.loading = true;
    var request = new GridRequest(new Grid());
    request.filters = this.filters;
    request.allData = true;
    request.searchTerm = this.searchTerm;
    this.api.student.getStudentAttendanceDetail(request).subscribe(result => {
      this.loading = false;
      this.grid = result;
    }, error => {
        this.loading = false;
    });
  }
}
