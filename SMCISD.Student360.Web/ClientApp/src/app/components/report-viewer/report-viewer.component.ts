import { Component, Input } from '@angular/core';
import { ApiService } from '@app/services/api/api.service';

@Component({
  selector: 'app-report-viewer',
  templateUrl: './report-viewer.component.html'
})

export class ReportViewerComponent {
  @Input() uri = '';

  reportServer: string = 'https://reports.smcisd.net/reportserver';
  reportUrl: string = '';
  showParameters: string = "false";
  parameters: any = {};
  language: string = "en-us";
  width: number = 50;
  height: number = 150;
  toolbar: string = "true";

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.reportUrl = this.apiService.reportViewer.getUrl();
  }
}
