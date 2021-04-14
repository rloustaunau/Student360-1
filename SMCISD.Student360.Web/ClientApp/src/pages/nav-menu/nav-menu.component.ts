import { Component, OnInit, OnDestroy } from '@angular/core';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';
import { AuthService, GoogleLoginProvider } from 'angularx-social-login';
import { ApiService } from '@app/services/api/api.service';
import { People } from '../../app/services/api/current-user.service';
import { GridFilter } from '../../app/components/data-grid/data-grid.component';
import { Subscription } from 'rxjs';
import { Token, AuthenticationService } from '../../app/services/authentication/authentication.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  isExpanded = false;
  people: People;
  filters: GridFilter[];
  validFilterUrls = ['/dnaChart', '/', '/hours', '/letters'];
  tokenSubscription: Subscription;
  token: Token;

  constructor(
    private router: Router,
    private socialAuthService: AuthService,
    private apiService: ApiService, private auth: AuthenticationService
  ) {
    this.people = new People;

    this.tokenSubscription = this.auth.tokenInfo.subscribe(result => {
      this.token = result;
    });
  }

  showFilters(): boolean {
    return this.validFilterUrls.some(x => x == this.router.url);
  }

  ngOnInit(): void {   

    this.token = this.auth.currentTokenValue;
    this.fetchData();

    this.router.events.pipe(
      filter((event: RouterEvent) => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.fetchData();
    });
  }

  fetchData() {

    var isAdmin = +this.token.level_id == 0;
    var adminColumns = ["IsHomeless", "EcoDis"];
    if (this.router.url == "/letters") {
     
      this.filters = [
        { value: undefined, column: "SchoolId", operator: "==", options: [], type: "select", serviceName: 'school', methodName: 'getSchools', defaultValue: true, placeholder: 'School', formatType: null, maxValue: null, description: 'School' },
        { value: undefined, column: "GradeLevel", operator: "==", options: [], type: "select", serviceName: null, methodName: null, defaultValue: false, placeholder: 'GradeLevel', formatType: null, maxValue: null, prefilledOptions: true, description: 'Grade Level' },
      ].filter(x => isAdmin ? true : !adminColumns.some(adminC => adminC == x.column));
    } else {
      this.filters = [ // The order affects how they are displayed in the html
        { value: undefined, column: "SchoolId", operator: "==", options: [], type: "select", serviceName: 'school', methodName: 'getSchools', defaultValue: true, placeholder: 'School', formatType: null, maxValue: null, description: 'School' },
        //{ value: undefined, column: "SchoolYear", operator: "==", options: [],  serviceName: 'school', methodName: 'getSchoolYears', defaultValue: true, placeholder: 'SchoolYear', formatType: null,  maxValue: null },
        { value: undefined, column: "GradeLevel", operator: "==", options: [], type: "select", serviceName: null, methodName: null, defaultValue: false, placeholder: 'GradeLevel', formatType: null, maxValue: null, prefilledOptions: true, description: 'Grade Level' },
        { value: undefined, column: "GraduationSchoolYear", operator: "==", options: [], type: "select", serviceName: 'school', methodName: 'getCohorts', defaultValue: false, placeholder: 'Cohort', formatType: null, maxValue: null, description: 'Cohort' },
        //{ value: undefined, column: "AbsencePercent", operator: ">=", options: [],  serviceName: 'school', methodName: 'getAbsencePercentList', defaultValue: false, placeholder: 'Absence %', formatType: 'percent',  maxValue: null, description: "Highest Absence Percentage of a student's class periods" },
        { value: undefined, column: "HighestCourseCount", operator: ">=", options: [], type: "autocomplete", serviceName: 'school', methodName: 'getAbsenceCountList', defaultValue: false, placeholder: 'Absences', formatType: null, maxValue: null, description: "Highest Absences of any student's class periods" },
        //{ value: 0, column: "HighestCourseCount", operator: ">=", options: [],  serviceName: null, methodName: null, defaultValue: true, placeholder: 'Abs #', formatType: null, isRange: true, maxValue: 172 },
        //{ value: 172, column: "HighestCourseCount", operator: "<=", options: [],  serviceName: null, methodName: null, defaultValue: false, placeholder: 'Abs #', formatType: null, isRange: true, maxValue: 172 },
        //{ value: 0, column: "AbsencePercent", operator: ">=", options: [],  serviceName: null, methodName: null, defaultValue: true, placeholder: 'Att %', formatType: 'percent', isRange: true, maxValue: 100 },
        //{ value: 100, column: "AbsencePercent", operator: "<=", options: [],  serviceName: null, methodName: null, defaultValue: false, placeholder: 'Att %', formatType: 'percent', isRange: true, maxValue: 100 },
        { value: undefined, column: "HasCredits", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: 'Credits', formatType: null, maxValue: null, description: 'Enrolled in a for-credit HS class' },
        { value: undefined, column: "Section504", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: '504', formatType: null, maxValue: null, description: 'Have a 504 plan' },
        { value: undefined, column: "Ell", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: 'Ell', formatType: null, maxValue: null, description: 'English Language Learners' },
        { value: undefined, column: "Ssi", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: 'SSI', formatType: null, maxValue: null, description: 'Failed gr. 5 or 8 STAAR' },
        { value: undefined, column: "Ar", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: 'AR', formatType: null, maxValue: null, description: 'Flagged at-risk' },
        { value: undefined, column: "IsHomeless", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: 'Homeless', formatType: null, maxValue: null, description: 'Flagged homeless' },
        { value: undefined, column: "Sped", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: 'SPED', formatType: null, maxValue: null, description: 'Special Education' },
        { value: undefined, column: "EcoDis", operator: "==", options: [], type: "bool", serviceName: null, methodName: null, defaultValue: false, placeholder: 'EcoDis', formatType: null, maxValue: null, description: 'Flagged EcoDis' },
      ].filter(x => isAdmin ? true : !adminColumns.some(adminC => adminC == x.column));
    }
    this.apiService.currentUser.getProfile().subscribe(
      result => {
        this.people = result;
      }
    );
  }

  clearFilters() {
    this.filters = this.filters.map(filter => {

      if (filter.type == "range" && filter.operator == ">=")
        filter.value = 0;
      else if (filter.type == "range" && filter.operator == "<=")
        filter.value = filter.maxValue;
      else if (filter.type == "bool")
        filter.value = false;
      else if (filter.defaultValue) {
        console.log(filter);
        filter.value = filter.options[0].id;
      }
      else
        filter.value = undefined;

      return filter;
    });

    this.apiService.filter.updateFilters(this.filters);

    this.fetchData();
  }

  updateFilter(event: any, index: number, isRange?: boolean) {
    let newFilters = this.filters.slice();
    if (isRange) {
      newFilters[index] = event.minFilter;
      newFilters[index + 1] = event.maxFilter;
    }
    else {
      newFilters[index] = event;
    }

    if (newFilters[index].column == "SchoolId") {
      var gradefilterIndex = 1;
      newFilters[gradefilterIndex].value = undefined;
      newFilters[gradefilterIndex].options = newFilters[index].options.find(x => x.id == newFilters[index].value).childOptions;
    }
    // Send update when at least every filter with default value has loaded
    if (newFilters.length == this.filters.length && newFilters.filter(x => x.value).length >= this.filters.filter(x => x.defaultValue).length)
      this.apiService.filter.updateFilters(newFilters);
    this.filters = newFilters;
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  signOut() {
    this.socialAuthService.signOut(true).then(result => { localStorage.clear(); this.router.navigate(['/login']); });
  }

  ngOnDestroy() {
    this.tokenSubscription.unsubscribe();
  }
}
