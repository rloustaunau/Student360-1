
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { GridFilter } from '../../components/data-grid/data-grid.component';

@Injectable({ providedIn: 'root' })
export class FilterService {
  private subject = new BehaviorSubject<GridFilter[]>([]);
  private filters: GridFilter[];
  public currentFilters = this.subject.asObservable();

    constructor() { }

  public updateFilters(filters: GridFilter[]) {
    this.filters = filters;
    this.subject.next(filters);
  }
  public getFilters(): GridFilter[] { return this.filters }
}
