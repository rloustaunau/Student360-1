import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { GridFilter, FilterOption } from '../data-grid/data-grid.component';
import { ApiService } from '../../services/api/api.service';

@Component({
  selector: 'app-filter-boolean',
  templateUrl: './filter-boolean.component.html',
  styleUrls: ['./filter-boolean.component.css']
})

export class FilterBooleanComponent implements OnInit {
    // Uncomment when loading from external controller
  
  @Output() valueChanged = new EventEmitter();
  @Input() filter: GridFilter;
    

  constructor(private apiService: ApiService) {
    this.filter = new GridFilter();
    this.filter.value = false;
  }

  ngOnInit(): void {
    
  }

  updateFilter() {
    this.filter.value = !this.filter.value;
    this.valueChanged.emit(this.filter);
  }
}

