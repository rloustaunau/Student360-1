import { Component, Input, OnInit } from '@angular/core';
import { GridFilter, GridRequest, Grid } from '../data-grid/data-grid.component';
import { ApiService } from '../../services/api/api.service';

@Component({
  selector: 'app-student-course-transcript',
  templateUrl: './student-course-transcript.component.html',
  styleUrls: ['./student-course-transcript.component.css']
})

export class StudentCourseTranscriptComponent implements OnInit {
  @Input() object: any;
  request: GridRequest;
  filters: GridFilter[];
  grid: Grid;
  loading: boolean = false;
  totalEarnedCredits = 0;
  totalAttemptedCredits = 0;

  constructor(private api: ApiService) {
    
  }

  ngOnInit(): void {
    this.request = {
      filters: [{ value: this.object['studentUsi'], column: "StudentUsi", operator: "==", options: [],  serviceName: null, methodName: null, defaultValue: true, placeholder: null, formatType: null, maxValue: null }],
      pageNumber: 1,
      pageSize: 25,
      allData: true,
      orderBy: undefined,
      select: [],
      searchTerm: null
    };
    this.loading = true;
    this.api.student.getStudentCourseTranscript(this.request).subscribe(result => {
      this.loading = false;
      this.grid = result;
      this.totalEarnedCredits = this.grid.data.reduce((a, b) => a + (b.earnedCredits || 0), 0);
      this.totalAttemptedCredits = this.grid.data.reduce((a, b) => a + (b.attemptedCredits || 0), 0);
    });
  }
}
