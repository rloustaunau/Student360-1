import { Component, Input, OnInit, AfterViewInit, ViewChild, ElementRef, SimpleChanges, OnChanges, ComponentRef, ViewContainerRef, ComponentFactoryResolver } from '@angular/core';
import { ApiService } from '../../services/api/api.service';
import { calculateHeaders, calculateOrderByForRequest } from './grid-helper';
@Component({
  selector: 'app-data-grid',
  templateUrl: './data-grid.component.html',
  styleUrls: ['./data-grid.component.css']
})

export class DataGridComponent implements OnInit, OnChanges {
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

  gridRequest: GridRequest;
  grid: Grid;
  loading: boolean;

  constructor(private apiService: ApiService) {
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

  searchChanged(searchTerm: string) {
    this.grid.pageNumber = 1;
    this.grid.searchTerm = searchTerm;
    this.sendRequest();
  }

  updatePageSize(pageSize: number) {
    this.grid.pageSize = +pageSize;
    this.sendRequest();
  }

  pageChanged(page) {
    this.grid.pageNumber = page;
    this.sendRequest();
  }

  // Grid helpers
  sendRequest(openDefault?: boolean) {
    this.loading = true;
    this.gridRequest = new GridRequest(this.grid);
    this.gridRequest.orderBy = calculateOrderByForRequest(this.grid);
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
          this.grid.headers = calculateHeaders(this.grid);
      },
      error => {
        this.loading = false;
      });
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
  metadata: any[];
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
