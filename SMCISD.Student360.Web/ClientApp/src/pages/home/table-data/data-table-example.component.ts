import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { ApiService } from '../../../app/services/api/api.service';
import { GridFilter } from '../../../app/components/data-grid/data-grid.component';

 @Component({
   selector: 'app-example',
   templateUrl: './data-table-example.component.html'
 })

 export class DataTableExampleComponent implements OnDestroy {
   subscription: Subscription;
   filters : GridFilter[];

   constructor(public apiService: ApiService) {
     this.filters = apiService.filter.getFilters();
     console.log(this.filters);
     this.subscription = apiService.filter.currentFilters.subscribe(result => {
       this.filters = result;
      });
   }

   ngOnDestroy(): void {
     this.subscription.unsubscribe();
   }
}
