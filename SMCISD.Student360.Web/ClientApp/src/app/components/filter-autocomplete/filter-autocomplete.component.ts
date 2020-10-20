import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { GridFilter, FilterOption } from '../data-grid/data-grid.component';
import { ApiService } from '../../services/api/api.service';

@Component({
  selector: 'app-filter-autocomplete',
  templateUrl: './filter-autocomplete.component.html',
  styleUrls: ['./filter-autocomplete.component.css']
})

export class FilterAutocompleteComponent implements OnInit {
    // Uncomment when loading from external controller
  
  @Input() serviceName: string;
  @Input() methodName: string;
  @Output() valueChanged = new EventEmitter();
  @Input() filter: GridFilter;
  @Input() defaultValue: boolean = false;
  @Input() formatType: string;
  @Input() preFilled: boolean = false;
  loading: boolean = false;
    

  constructor(private apiService: ApiService) {
    this.filter = new GridFilter();
  }

  ngOnInit(): void {
    if (!this.preFilled) {
      this.loading = true;
      this.apiService[this.serviceName][this.methodName]().subscribe(
        result => {
          this.filter.options = this.mapListToOptions(result);
          if (this.defaultValue) {
            this.filter.value = "" + this.filter.options[0].id; // By default select index 0 as a value
            this.updateFilter();
          }
          this.loading = false;
        },
        error => {
          this.loading = false;
        });
    }
  }

  updateFilter() {
      this.valueChanged.emit(this.filter);
  }

  mapListToOptions(list): FilterOption[] {
    if (list.length == 0)
      return [];

    var object = list[0];
    
    var keys = Object.keys(object).filter(x => x != "localEducationAgencyId");
    var childKey = keys.find(x => x.includes("child"));
    if (childKey)
      keys = keys.filter(x => x != childKey);
    var options = [];

    if (keys.length == 0) {
      list.forEach(item => {
          options.push({ id: item, value: item }); // We will need to check this
      });
      return options;
    }
    if (keys.length > 1) {
      // Check for Column Id
      var idKey = keys.find(key => key.includes("Id") || key.includes("id"));

      list.forEach(item => {
        if (idKey != null)
          options.push({ id: item[idKey], value: item[keys.find(x => x != idKey)], childOptions : item[childKey] });
        else
          options.push({ id: item[keys[0]], value: item[keys[1]], childOptions: item[childKey] }); // We will need to check this
      });

    } else {
      // Just has value
      list.forEach(item => {
        options.push({ id: item[keys[0]], value: item[keys[0]] });
      });
    }
    return options;
  }

  getFormatValue(value: string) : string {
    switch (this.formatType) {
      case 'percent':
        return value + '%';
      default:
        return value;
    }
  }

   
}

