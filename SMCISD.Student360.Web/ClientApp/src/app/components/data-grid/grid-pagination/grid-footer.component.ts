import { Component, Input, OnInit, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';

@Component({
  selector: 'app-grid-footer',
  templateUrl: './grid-footer.component.html',
  styleUrls: ['./grid-footer.component.css']
})

export class GridFooterComponent implements OnInit {
  // Uncomment when loading from external controller
  @Input() pageNumber: number;
  @Input() pageSize: number;
  @Input() filteredCount: number;
  @Input() totalCount: number;
  @Input() maxPageNumber: number;
  @Input() showPagination = true;
  @Output() pageChanged = new EventEmitter();


  constructor() {
  }

  ngOnInit() {
    console.log(this.pageNumber);
    console.log(this.maxPageNumber);
  }

  pageSelected(page) {
    this.pageNumber = page;
    this.pageChanged.emit(page);
  }

  calculateFirstPage() {
    return this.pageNumber % (2 + (Math.ceil(this.pageNumber / 3) - 1) * 3) == 0 ? this.pageNumber - 1 : this.pageNumber % 3 == 0 ? this.pageNumber - 2 : this.pageNumber;
  }

  calculateSecondPage() {
    return this.pageNumber % (2 + (Math.ceil(this.pageNumber / 3) - 1) * 3) == 0 ? this.pageNumber : this.pageNumber % 3 == 0 ? this.pageNumber - 1 : this.pageNumber + 1;
  }

  calculateThirdPage() {
    return this.pageNumber % (2 + (Math.ceil(this.pageNumber / 3) - 1) * 3) == 0 ? this.pageNumber + 1 : this.pageNumber % 3 == 0 ? this.pageNumber : this.pageNumber + 2;
  }
}
