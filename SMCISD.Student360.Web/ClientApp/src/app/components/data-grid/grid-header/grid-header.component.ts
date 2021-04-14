import { Component, Input, OnInit, Output, ViewChild, ElementRef } from '@angular/core';
import * as moment from 'moment';
import { EventEmitter } from '@angular/core';
import { GridRequest, Grid, GridFilter } from '../data-grid.component';
import { calculateOrderByForRequest } from '../grid-helper';
import { ApiService } from '../../../services/api/api.service';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MessageModalComponent } from '../../modals/message-modal/message-modal.component';
import { filter, distinctUntilChanged, tap, debounceTime } from 'rxjs/operators';
import { fromEvent } from 'rxjs';
import { downloadCsv } from '@app/components/shared/file.helper';

@Component({
  selector: 'app-grid-header',
  templateUrl: './grid-header.component.html',
  styleUrls: ['./grid-header.component.css']
})

export class GridHeaderComponent implements OnInit {
  // Uncomment when loading from external controller
  @Input() grid: Grid;
  @Input() showPagination = true;
  @Input() showImportExport = false;
  @Input() selectedColumns: string[] = [];
  @Input() serviceName: string;
  @Input() showSearch: boolean = true;
  @Input() methodName: string;
  @Input() filters: GridFilter[];
  loading = false;
  @Output() pageSizeChanged = new EventEmitter<number>();
  @Output() searchTermChanged = new EventEmitter<string>();
  @ViewChild('searchInput', null) searchInput: ElementRef;
  constructor(private apiService: ApiService, private toastr: ToastrService, private modalService: NgbModal) {
  }

  ngOnInit() {

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

  search(searchTerm: string) {
    this.grid.pageNumber = 1;
    this.grid.searchTerm = searchTerm;
    this.searchTermChanged.emit(searchTerm);
  }

  updatePageSize() {
    this.pageSizeChanged.emit(this.grid.pageSize)
  }

  exportCsv() {
    this.loading = true;
    this.toastr.info('The file is beeing processed, it will take a few seconds.');
    console.log(this.grid);
    var request = new GridRequest(this.grid);
    request.allData = true;
    request.orderBy = calculateOrderByForRequest(this.grid);
    request.select = this.selectedColumns;
    this.apiService[this.serviceName][this.methodName](request).subscribe(
      result => {
        this.loading = false;
        downloadCsv(this.generateCsv(result.data), this.calculateGridFileName());
      },
      error => {
        this.loading = false;
      });
  }

  generateCsv(data: any[], conflict?: boolean): string {
    var headers = ['StudentUniqueId', 'FirstName', 'LastSurname', 'GradeLevel', 'Date', 'Hours', 'Reason', 'Comments'];
    var rows = data.map(x => {
      if (conflict)
        return [x.studentUniqueId, x.firstName, x.lastSurname, x.gradeLevel, '', x.hours, x.reason.value, x.comments];
      else
        return [x.studentUniqueId, x.firstName, x.lastSurname, x.gradeLevel, '', '', '', ''];
    });

    var fileData = [];
    fileData.push(headers);
    fileData.push(...rows);

    return "data:text/csv;charset=utf-8," + fileData.map(x => x.join(',')).join('\n');
  }

  calculateGridFileName(title?: string) {

    return title ? title : this.filters.filter(x => !(x.value === undefined || x.value === null)).map(x => {
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
            if (!cols.every(col => col == null)) {

              var recordDate = cols[4] ? new Date(cols[4]) : new Date();
              result.push(
                {
                  studentUniqueId: cols[0],
                  firstName: cols[1],
                  lastSurname: cols[2],
                  gradeLevel: cols[3],
                  date: new Date(recordDate.getTime() - recordDate.getTimezoneOffset() * -60000),
                  hours: cols[5] ? +cols[5] : 0,
                  reason: { value: cols[6] },
                  comments: cols[7],
                  userRole: null,
                  userCreatedUniqueId: null
                }
              );
            }

          });
          this.importFile(result);
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
        downloadCsv(this.generateCsv(result, true), this.calculateGridFileName('Data_Conflicts'));
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
      'One or more required fields is blank. ( StudentUniqueID / Date / Hours / Reason ).',
      'The "Reason" is invalid, and must match predefined reasons that enable hours.',
      'The quantity of hours must be greater than 0 (zero).',
      'The "Date" is in the future.',
      'The "Date" is prior to the current school year.',
      'An entry with the same StudentUniqueID, Date, and Reason already exists.'
    ]
  }
}
