import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { GridFilter, FilterOption } from '../data-grid/data-grid.component';
import { ApiService } from '../../services/api/api.service';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-bold-report-viewer',
  templateUrl: './bold-report-viewer.component.html',
  styleUrls: ['./bold-report-viewer.component.css']
})

export class BoldReportViewerComponent {
  @Input() serviceUrl: string;
  @Input() reportPath: string;
    

  constructor(private apiService: ApiService) {
    this.serviceUrl = `${environment.apiUrl}/api/ReportViewer`;
    this.reportPath = '/Resources/sales-order-detail.rdl';
  }
}

