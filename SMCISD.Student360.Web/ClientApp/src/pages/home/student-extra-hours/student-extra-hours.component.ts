import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { ApiService } from '../../../app/services/api/api.service';
import { GridFilter } from '../../../app/components/data-grid/data-grid.component';
import { ActivatedRoute } from '@angular/router';

 @Component({
   selector: 'app-student-extra-hours',
   templateUrl: './student-extra-hours.component.html'
 })

 export class StudentExtraHoursComponent implements OnInit, OnDestroy {
   subscription: Subscription;
   filters: GridFilter[];
   availableFilterColumns: string[];
   studentUniqueId: string;

   constructor(public apiService: ApiService, private route: ActivatedRoute) {
     this.availableFilterColumns = ['SchoolId','GradeLevel']; // using the properties to know which filters apply to the grid
     this.filters = this.getApplicableFilters(apiService.filter.getFilters());
     this.subscription = apiService.filter.currentFilters.subscribe(result => {
       this.filters = this.getApplicableFilters(result);
     });
     this.studentUniqueId = this.route.snapshot.paramMap.get('id');
   }

   ngOnInit() {
    
   }
   getApplicableFilters(filters: GridFilter[]) {
     if (!filters)
       return [];
     return filters.filter(x => this.availableFilterColumns.some(key => key.toUpperCase() == x.column.toUpperCase()));
   }

   ngOnDestroy(): void {
     this.subscription.unsubscribe();
   }
}
