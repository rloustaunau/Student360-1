import { Component } from '@angular/core';
import { ApiService } from '@app/services/api/api.service';
import { Report } from '@app/services/api/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-report-list',
  templateUrl: './report-list.component.html',
  styleUrls: ['./report-list.component.css']
})

export class ReportListComponent {

  public reports: Report[];

  constructor(private apiService: ApiService, private router: Router) {
  }

  ngOnInit() {
    this.apiService.report.getSsrsReports().subscribe(result => {
      this.reports = result; debugger;
    });
  }

  updateUrl(report: Report): void {
    this.apiService.reportViewer.updateUrl(report.reportUri);
    this.router.navigateByUrl("report/" + report.reportUri);
  }
}
