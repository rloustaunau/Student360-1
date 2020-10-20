import { Component, Input, OnInit, EventEmitter, Output, AfterViewInit, AfterContentInit, AfterViewChecked, NgZone } from '@angular/core';
import { GridFilter } from '../data-grid/data-grid.component';
import * as $ from 'jquery';

@Component({
  selector: 'app-filter-range',
  templateUrl: './filter-range.component.html',
  styleUrls: ['./filter-range.component.css']
})

export class FilterRangeComponent implements OnInit, AfterViewInit {
  @Output() valueChanged = new EventEmitter();
  @Input() minFilter: GridFilter = new GridFilter();
  @Input() maxFilter: GridFilter = new GridFilter();
  @Input() maxValue: number = 1000;
  @Input() label: string = "";
  @Input() inputId: number = 0;
  @Input() formatType: string = "";
  minValue = 0;
  differenceThreshold = 10;
  maxId : string;
  minId : string;
  sliderId: string;
  colorlineId: string;
  sliderWidth: number = 100;
  maxRawValue = 100;
  minRawValue = 0;
  pixelOffset = 5;
  constructor(private zone: NgZone) {
  
  }

  ngOnInit(): void {
    this.minFilter.value = this.minValue;
    this.maxFilter.value = this.maxValue;
    this.maxId = '#max' + this.inputId;
    this.minId = '#min' + this.inputId;
    this.sliderId = '#slider' + this.inputId;
    this.colorlineId = '#color' + this.inputId;
  }

  ngAfterViewInit(): void {
    this.initSliders();
  }

  calculateFilterValues() {
    this.zone.run(() => {
      this.minFilter.value = Math.round((this.minRawValue / this.sliderWidth) * this.maxValue);
      this.maxFilter.value = Math.round((this.maxRawValue / this.sliderWidth) * this.maxValue);
    });
  }

  updateFilter() {
    this.zone.run(() => {
      this.valueChanged.emit({ minFilter: this.minFilter, maxFilter: this.maxFilter });
    });
  }

  adjustMinRange() {
    var diff = Math.round((this.minFilter.value / this.maxValue) * 100);
    this.minRawValue = diff;
    $(this.minId).css('left', diff + 'px');
    $(this.colorlineId).css('left', diff + 'px');
    $(this.colorlineId).css('width', (this.maxRawValue - this.minRawValue) + 'px');
  }

  adjustMaxRange() {
    var diff = Math.round((this.maxFilter.value / this.maxValue) * 100);
    this.maxRawValue = diff;
    $(this.maxId).css('left', diff + 'px');
    $(this.colorlineId).css('width', (this.maxRawValue - this.minRawValue) + 'px');
  }

  initSliders() {
    var that = this;
    $(document).ready(function () {
      $(that.minId).bind('mousedown', function (e) {
        $(that.sliderId).bind('mousemove', function (e) {
          var diff = e.pageX - $(that.sliderId).offset().left - that.pixelOffset;
          if (diff <= 0)
            diff = 0;
          if (diff >= (that.maxRawValue - that.pixelOffset)) // has to be the less than the current max value
            diff = that.maxRawValue - that.pixelOffset;
          that.minRawValue = diff;
          $(that.minId).css('left', diff + 'px');
          $(that.colorlineId).css('left', diff + 'px');
          $(that.colorlineId).css('width', (that.maxRawValue - that.minRawValue) + 'px');
          that.calculateFilterValues();
        });
        $('body').bind('mouseup', function (e) {
          $(that.sliderId).unbind('mousemove');
          $('body').unbind('mouseup');
          that.updateFilter();
        });
      });

      //$(that.maxId).bind('mousedown', function (e) {
      //  $(that.sliderId).bind('mousemove', function (e) {
      //    var diff = e.pageX - $(that.sliderId).offset().left - that.pixelOffset;
      //    if (diff <= (that.minRawValue + that.pixelOffset)) // has to be greater than current min value
      //      diff = that.minRawValue + that.pixelOffset;
      //    if (diff >= that.sliderWidth) // has to be the same width as the #slider
      //      diff = that.sliderWidth;
      //    that.maxRawValue = diff;
      //    $(that.maxId).css('left', diff + 'px');
      //    $(that.colorlineId).css('width', (that.maxRawValue - that.minRawValue) + 'px');
      //    that.calculateFilterValues();
      //  });
      //  $('body').bind('mouseup', function (e) {
      //    $(that.sliderId).unbind('mousemove');
      //    $('body').unbind('mouseup');
      //    that.updateFilter();
      //  });
      //});
    });
  }
}

