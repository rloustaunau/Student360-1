import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Subscription, fromEvent } from 'rxjs';
import { ApiService } from '../../../app/services/api/api.service';
import { GridFilter, Grid, GridRequest } from '../../../app/components/data-grid/data-grid.component';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { StudentExtraHourGrid } from '../../../app/services/api/student-extra-hour.service';

@Component({
  selector: 'app-student-extra-hours',
  templateUrl: './student-extra-hours.component.html'
})

export class StudentExtraHoursComponent implements OnInit, OnDestroy {
  subscription: Subscription;
  filters: GridFilter[];
  gridRequest: GridRequest;
  availableFilterColumns: string[];
  studentUniqueId: string;
  loading = false;
  schoolYearFilter: GridFilter;
  schoolYears: number[] = [];
  grid: Grid;
  @ViewChild('searchInput', null) searchInput: ElementRef;

  constructor(public apiService: ApiService, private route: ActivatedRoute,
    private toastr: ToastrService, private modalService: NgbModal) {
    this.schoolYearFilter = {
      value: undefined, column: "SchoolYear", operator: "==", type: "select",
      defaultValue: false, formatType: null, maxValue: null, placeholder: "School Year", methodName: 'getSchoolYears',
      serviceName: 'school', options: []
    };
    this.grid = new Grid();
    this.availableFilterColumns = ['SchoolId', 'GradeLevel']; // using the properties to know which filters apply to the grid
    this.filters = this.getApplicableFilters(apiService.filter.getFilters());
    this.subscription = apiService.filter.currentFilters.subscribe(result => {
      this.filters = this.getApplicableFilters(result);
      this.getData();
    });
    this.studentUniqueId = this.route.snapshot.paramMap.get('id');
    if (this.studentUniqueId)
      this.grid.searchTerm = this.studentUniqueId;
  }

  ngOnInit() {
    this.getSchoolYears();
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

  getApplicableFilters(filters: GridFilter[]) {
    if (!filters)
      return [];
    var applicableFilters = filters.filter(x => x.value && this.availableFilterColumns.some(key => key.toUpperCase() == x.column.toUpperCase()));
    applicableFilters.push(this.schoolYearFilter);
    return applicableFilters;
  }


  getSchoolYears() {
    this.loading = true;
    this.apiService.school.getSchoolYears().subscribe(result => {
      this.schoolYears = result.map(x => x.schoolYear);
      this.schoolYearFilter.value = this.schoolYears[0];
      this.getData();
    });
  }

  getData() {
    this.loading = true;
    this.grid.filters = this.getApplicableFilters(this.filters);
    this.gridRequest = new GridRequest(this.grid);
    this.apiService.studentExtraHour.getStudentExtraHourGrid(this.gridRequest).subscribe(result => {
      this.loading = false;
      this.grid.data = result.data;
      this.grid.filteredCount = result.filteredCount;
      this.grid.maxPageNumber = Math.ceil(this.grid.filteredCount / this.grid.pageSize);
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
}
