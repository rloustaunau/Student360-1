import { Component, Input, OnInit, AfterViewInit, ViewChild, ElementRef, SimpleChanges, OnChanges, ComponentRef, ViewContainerRef, ComponentFactoryResolver } from '@angular/core';
import { filter, distinctUntilChanged, tap, debounceTime } from 'rxjs/operators';
import { fromEvent } from 'rxjs';
import { ApiService } from '../../services/api/api.service';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MessageModalComponent } from '../modals/message-modal/message-modal.component';
import * as moment from 'moment';

@Component({
  selector: 'app-data-grid',
  templateUrl: './data-grid.component.html',
  styleUrls: ['./data-grid.component.css']
})

export class DataGridComponent implements OnInit, AfterViewInit, OnChanges {
  // Uncomment when loading from external controller
  @Input() serviceName: string;
  @Input() showSearch: boolean = true;
  @Input() methodName: string;
  @Input() filters: GridFilter[];
  @Input() selectedColumns: string[] = [];
  @Input() title: string;
  @Input() subtitle: string;
  @Input() drilldownComponent: string;
  @Input() hiddenColumns: string[] = [];
  @Input() stringShortenOptions: StringShorten[] = [];
  @Input() renameHeaders: HeaderDisplay[] = [];
  @Input() defaultSearch: string;
  @Input() showImportExport = false;
  @Input() showPagination = true;

  @ViewChild('searchInput', null) searchInput: ElementRef;
  gridRequest: GridRequest;
  grid: Grid;
  loading: boolean;

  constructor(private apiService: ApiService, private toastr: ToastrService, private modalService: NgbModal) {
    this.grid = new Grid();
    if (this.filters)
      this.grid.filters = this.filters;
  }

  ngOnInit() {
    if (this.defaultSearch) {
      this.grid.searchTerm = this.defaultSearch.trim();
      this.sendRequest(true);
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.filters) {
      this.grid.filters = changes.filters.currentValue;
      this.grid.pageNumber = 1;
      this.sendRequest();
    }
  }

  ngAfterViewInit(): void {
    if (this.showSearch) {
      fromEvent(this.searchInput.nativeElement, 'keyup')
        .pipe(
          filter(Boolean),
          debounceTime(1500),
          distinctUntilChanged(),
          tap((text) => {
            this.search(this.searchInput.nativeElement.value);
          })
        )
        .subscribe();
    }
  }

  getShortenOption(index: number) {
    var shorten = this.stringShortenOptions.find(x => x.index == index);
    return shorten ? shorten.option : undefined;
  }

  getStringReplace(index: number, current: string) {
    var replace = this.renameHeaders.find(x => x.index == index);
    return replace ? replace.name : current;
  }

  // Grid methods
  sortGrid(header: GridHeader) {
    this.grid.pageNumber = 1;
    if (header.order === false) {
      header.order = undefined;
      header.orderNumber = 0;
    }
    else if (header.order !== undefined)
      header.order = !header.order;
    else
      header.order = true;

    if (!header.orderNumber)
      header.orderNumber = Math.max.apply(null, this.grid.headers.map(x => x.orderNumber)) + 1;

    this.sendRequest();
  }

  search(searchTerm: string) {
    this.grid.pageNumber = 1;
    this.grid.searchTerm = searchTerm;
    this.sendRequest();
  }

  pageSelected(page: number) {
    this.grid.pageNumber = page;
    this.sendRequest();
  }

  updatePageSize() {
    this.sendRequest();
  }

  // Grid helpers
  sendRequest(openDefault?: boolean) {
    this.loading = true;
    this.gridRequest = new GridRequest(this.grid);
    this.gridRequest.orderBy = this.calculateOrderByForRequest();
    this.gridRequest.select = this.selectedColumns;
    this.apiService[this.serviceName][this.methodName](this.gridRequest).subscribe(
      result => {
        this.loading = false;

        this.grid.data = result.data;

        this.grid.totalCount = result.totalCount;
        this.grid.filteredCount = result.filteredCount;
        this.grid.columns = result.data.length > 0 ? Object.keys(result.data[0]).filter(x => !this.hiddenColumns.some(col => col == x)) : [];
        this.grid.maxPageNumber = Math.ceil(this.grid.filteredCount / this.grid.pageSize);

        if (openDefault && this.grid.totalCount > 0)
          this.grid.data[0].showDrilldown = true;

        if (this.grid.headers.length == 0)
          this.grid.headers = this.calculateHeaders();
      },
      error => {
        this.loading = false;
      });
  }

  calculateHeaders(): GridHeader[] {
    var headers = [];
    this.grid.columns.forEach(col => {
      headers.push({ name: this.camelCaseToSentence(col), order: undefined, columnName: this.camelCaseToPascalCase(col), orderNumber: 0 });
    });
    return headers;
  }


  calculateOrderByForRequest() {
    var result = [];

    var sortedHeaders = this.grid.headers.slice().sort((a, b) => {
      if (a.orderNumber > b.orderNumber)
        return 1;
      if (b.orderNumber > a.orderNumber)
        return -1;

      return 0;
    });

    sortedHeaders.forEach(header => {
      if (header.order !== undefined) {
        result.push({ column: header.columnName, direction: header.order ? 'descending' : 'ascending' });
      }
    });

    return result;
  }

  camelCaseToPascalCase(camelCaseWord: string) {
    return camelCaseWord.charAt(0).toUpperCase() + camelCaseWord.slice(1);
  }

  camelCaseToSentence(camelCaseWord: string) {
    var result = camelCaseWord.replace(/([A-Z])/g, " $1");
    return this.camelCaseToPascalCase(result);
  }

  calculateFirstPage() {
    return this.grid.pageNumber % (2 + (Math.ceil(this.grid.pageNumber / 3) - 1) * 3) == 0 ? this.grid.pageNumber - 1 : this.grid.pageNumber % 3 == 0 ? this.grid.pageNumber - 2 : this.grid.pageNumber;
  }

  calculateSecondPage() {
    return this.grid.pageNumber % (2 + (Math.ceil(this.grid.pageNumber / 3) - 1) * 3) == 0 ? this.grid.pageNumber : this.grid.pageNumber % 3 == 0 ? this.grid.pageNumber - 1 : this.grid.pageNumber + 1;
  }

  calculateThirdPage() {
    return this.grid.pageNumber % (2 + (Math.ceil(this.grid.pageNumber / 3) - 1) * 3) == 0 ? this.grid.pageNumber + 1 : this.grid.pageNumber % 3 == 0 ? this.grid.pageNumber : this.grid.pageNumber + 2;
  }

  exportCsv() {
    this.loading = true;
    this.gridRequest = new GridRequest(this.grid);
    this.gridRequest.allData = true;
    this.gridRequest.orderBy = this.calculateOrderByForRequest();
    this.gridRequest.select = this.selectedColumns;
    this.apiService[this.serviceName][this.methodName](this.gridRequest).subscribe(
      result => {
        this.loading = false;
        this.downloadCsv(this.generateCsv(result.data));
      },
      error => {
        this.loading = false;
      });
  }
  generateCsv(data: any[], conflict?: boolean): string {
    var headers = ['StudentUniqueId', 'FirstName', 'LastSurname', 'GradeLevel', 'SchoolYear', 'Date', 'Hours', 'Reason', 'Comments'];
    var rows = data.map(x => {
      if (conflict)
        return [x.studentUniqueId, x.firstName, x.lastSurname, x.gradeLevel, x.schoolYear, x.date, x.hours, x.reason.value, x.comments];
      else
        return [x.studentUniqueId, x.firstName, x.lastSurname, x.gradeLevel, x.schoolYear, '', '', '',''];
    });

    var fileData = [];
    fileData.push(headers);
    fileData.push(...rows);

    return "data:text/csv;charset=utf-8," + fileData.map(x => x.join(',')).join('\n');
  }

  downloadCsv(csv: string, title?: string) {
    var encodedUri = encodeURI(csv);
    var link = document.createElement("a");
    link.setAttribute("href", encodedUri);


    var fileName = title ? title : this.filters.filter(x => !(x.value === undefined || x.value === null)).map(x => {
      if (x.type == 'bool') {
        return x.value ? x.placeholder : '';
      }
      else if (x.type == 'range') {
        if (x.operator == ">=") {
          return x.placeholder + '_' + x.value + '_to';
        } else {
          return x.value;
        }
      }
      else {
        return x.options.find(o => o.id == x.value) ? x.options.find(o => o.id == x.value).value : x.value;
      }
    }).join('_').replace(/\s/g, '');

    link.setAttribute("download", fileName + '_' + new Date().toLocaleDateString("en-US") + ".csv"); // There are 2 types of spaces that is why there is 2 replace methods
    document.body.appendChild(link);
   
    link.click();
  }
  importCsv(files: FileList) {
    if (files && files.length > 0) {
      let file: File = files[0];
      let reader: FileReader = new FileReader();
      reader.readAsText(file);
      reader.onload = (e) => {
        let csv: string = reader.result as string;
        var rows = csv.split(/\r?\n/)
        var result = [];
        if (rows.shift().split(',').some(x => x.toUpperCase() == 'ID')) {
          this.toastr.warning('This file is not on the right format.');
        }
        else {
          rows = rows.filter(row => !!row);
          rows.forEach(row => {
            var cols = row.split(',');
            if (!cols.every(col => col == null))
              result.push({ studentUniqueId: cols[0], firstName: cols[1], lastSurname: cols[2], gradeLevel: cols[3], schoolYear: cols[4] ? +cols[4] : 0, date: cols[5]? new Date(cols[5]) : new Date(), hours: cols[6]? +cols[6] : 0, reason: { value: cols[7] }, comments: cols[8], userRole: null, userCreatedUniqueId: null });
          });
          if (result.some(x => {
            var end = moment(x.date);
            var start = moment(new Date());
            var hours = moment.duration(end.diff(start)).hours();
            return hours > 24;
          }))
            this.toastr.warning('This file has future dates, please wait until it is the current day.');
          else {
            this.importFile(result);
          }
        }
      }
    }
    else {
      this.toastr.warning('No file was selected');
    }
  }

  importFile(data: any[]) {
    this.apiService.studentExtraHour.importStudentExtraHours(data).subscribe(result => {
      this.toastr.success('The file has been imported.');
      if (result.length > 0) {
        this.openMessageModal('A file has been downloaded with the records that were ignored. Please check the following: ');
        this.downloadCsv(this.generateCsv(result, true), 'Data_Conflicts');
      }

    }, error => {
      this.toastr.error('There was an error with the file.');
    });
  }

  openMessageModal(message: string) {
    const modalRef = this.modalService.open(MessageModalComponent);
    modalRef.componentInstance.title = 'Data Conflict';
    modalRef.componentInstance.message = message;
    modalRef.componentInstance.reasons = [
      'There are some empty fields in the row',
      'The reason is invalid, contact the database manager',
      'The date is too far in the future, please use past dates or the current date',
      'The record with the StudentID, Date and Reason already exists'
    ]
  }
}

export class GridRequest {

  constructor(grid: Grid) {
    this.pageNumber = grid.pageNumber;
    this.pageSize = grid.pageSize;
    this.pageNumber = grid.pageNumber;
    this.searchTerm = grid.searchTerm;
    this.filters = grid.filters.filter(x => x.value && x.value != "undefined");
    this.allData = false;
  }

  pageNumber: number;
  pageSize: number;
  orderBy: any[];
  searchTerm: string;
  filters: GridFilter[];
  select: string[];
  allData: boolean;
}

export class Grid {

  constructor() {
    this.pageNumber = 1;
    this.pageSize = 25;
    this.searchTerm = "";
    this.columns = [];
    this.data = [];
    this.headers = [];
    this.filters = [];
  }

  data: any[];
  totalCount: number;
  filteredCount: number;
  queryExcecutionMs: number;
  filters: GridFilter[];
  columns: string[];
  headers: GridHeader[];
  pageSize: number;
  pageNumber: number;
  searchTerm: string;
  maxPageNumber: number;
}

export class GridHeader {
  name: string;
  columnName: string;
  order: boolean; // false = ascending, true = descending
  orderNumber: number;
  option: string;
}

export class GridFilter {

  constructor() {
    this.operator = "==";
  }

  column: string;
  operator: string;
  value: any;
  options: FilterOption[];
  serviceName: string;
  methodName: string;
  placeholder: string;
  defaultValue: boolean;
  formatType: string;
  type?: string;
  maxValue: any;
  prefilledOptions?: boolean;
  description?: string;
}

export class FilterOption {
  id: any;
  value: any;
  childOptions?: FilterOption[];
}

export class StringShorten {
  option: string;
  index: number;
}

export class HeaderDisplay {
  index: number;
  name: string;
}
