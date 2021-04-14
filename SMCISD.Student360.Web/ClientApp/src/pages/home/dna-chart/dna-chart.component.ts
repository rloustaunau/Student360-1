/// <reference types="@types/googlemaps" />
import { Component, Input, NgZone, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import * as moment from 'moment';
import { Subscription } from 'rxjs';
import { GridFilter, GridRequest, Grid } from '../../../app/components/data-grid/data-grid.component';
import { ApiService } from '../../../app/services/api/api.service';
import { StudentAbsencesCodesByPeriod, AbsencesCodesByPeriod, GeneralStudentDnaData } from '../../../app/services/api/student.service';
import { filter, distinctUntilChanged, tap, debounceTime } from 'rxjs/operators';
import { fromEvent } from 'rxjs';

@Component({
  selector: 'app-dna-chart',
  templateUrl: './dna-chart.component.html',
  styleUrls: ['./dna-chart.component.css']
})

export class DnaChartComponent implements AfterViewInit {
  loading = false;
  data: any;
  searchTerm: string;
  map: google.maps.Map;
  @ViewChild('map', { static: true }) mapElement: any;
  markers: MyMarker[] = [];
  selectedMarker: MyMarker;
  @Input() filters: GridFilter[];
  subscription: Subscription;
  nonValidFilters = ['HasCredits', 'Section504', 'AbsencePercent'];
  coordinatesHelper = {};
  noGeoDataStudents = 0;
  currentStudentData: GeneralStudentDnaData;
  @ViewChild('searchInput', null) searchInput: ElementRef;
  mapOptions: google.maps.MapOptions = {
    center: new google.maps.LatLng(29.880162, -97.940222),
    zoom: 14,
    disableDefaultUI: true,
    streetViewControl: false,
    mapTypeId: google.maps.MapTypeId.ROADMAP,
    draggable: true,
    zoomControl: true,
    scrollwheel: true,
    disableDoubleClickZoom: true,
    styles: [
      { featureType: 'poi', stylers: [{ visibility: 'off' }] },
      { featureType: 'transit', stylers: [{ visibility: 'off' }] },
      { featureType: 'landscape.natural', stylers: [{ visibility: 'off' }] }
    ]
  };



  constructor(private apiService: ApiService, private zone: NgZone) {
    this.data = [];
    this.currentStudentData = new GeneralStudentDnaData;
    this.filters = apiService.filter.getFilters();
    this.subscription = apiService.filter.currentFilters.subscribe(result => {
      this.filters = result;
      if (this.filters.length > 11)
        this.sendRequest();
    });
  }

  ngOnInit() {
    this.map = new google.maps.Map(this.mapElement.nativeElement, this.mapOptions);
  }

  ngAfterViewInit(): void {
    fromEvent(this.searchInput.nativeElement, 'keyup')
      .pipe(
        filter(Boolean),
        debounceTime(1500),
        distinctUntilChanged(),
        tap((text) => {
          this.search(this.searchInput.nativeElement.value);
        })
      )
      .subscribe();
  }

  search(searchTerm: string) {
    this.sendRequest(searchTerm);
  }
  getStudentAbsencesCodesByPeriodData(studentUsi: number, marker: MyMarker) {
    this.apiService.student.getStudentAbsencesCodesByPeriod(studentUsi).subscribe(result => {
      this.currentStudentData = result;
      marker.infoWindow = new google.maps.InfoWindow({ content: this.getInfoForWindow(marker.student, this.currentStudentData) });
      marker.infoWindow.open(this.map, marker.marker);
      if (this.selectedMarker)
        this.selectedMarker.infoWindow.close();
      this.selectedMarker = marker;
    });
  }
  createMarker(studentUniqueId: string, studentName: string, latitude: number, longitude: number) {
    const location = new google.maps.LatLng(latitude, longitude);
    return new google.maps.Marker({
      position: location,
      map: this.map,
      title: "ID: " + studentUniqueId.trim() + ". " + studentName,
      icon: { url: './assets/reddot.png', scaledSize: new google.maps.Size(10, 10) },
    });
  }

  calculateTransparency(daysFromLastAbsence: number): string {
    // Will be based on hex transparency
    const lowerHexLimit = 30;
    const maxHexLimit = 80;
    const maxAbsenceLimit = 100;

    if (daysFromLastAbsence >= maxAbsenceLimit)
      return maxHexLimit.toString();

    var calculatedHex = Math.round((maxHexLimit / maxAbsenceLimit) * daysFromLastAbsence);

    if (calculatedHex < lowerHexLimit)
      return lowerHexLimit.toString();
  }

  initMarkers() {
    const that = this;
    this.data.forEach(student => {
      var studentName = `${student.firstName} ${(student.middleName ? (student.middleName + ' ') : '')} ${student.lastSurname}`;
      const marker = this.createMarker(student.studentUniqueId, studentName, student.latitude, student.longitude);
      const transparency = this.calculateTransparency(student.daysFromLastAbsence);
      const circle = new google.maps.Circle({
        map: this.map,
        radius: 5 * student.highestCourseCount,  // meters in radius
        fillColor: '#f07474' + transparency,
        strokeColor: '#e8373780',
      });
      const myMarker = { marker: marker, student: student, infoWindow: null, clickListener: null, circle: circle };

      circle.bindTo('center', myMarker.marker, 'position');

      myMarker.marker.addListener('click', function () {
        that.getStudentAbsencesCodesByPeriodData(myMarker.student.studentUsi, myMarker);
        //that.zone.run(() => {

        //});
      });

      this.markers.push(myMarker);
    });
  }


  getInfoForWindow(student, generalData: GeneralStudentDnaData) {

    var codesAndQuantities: AbsencesCodesByPeriod[] = [].concat.apply([], generalData.periods.map(x => x.absenceCodes));
    var tableData: AbsencesCodesByPeriod[] = [];

    var codes = {};
    codesAndQuantities.forEach(x => {
      codes[x.absenceCode] = { description: x.description, absenceCode: x.absenceCode };
    });

    tableData = Object.keys(codes).map(key => {
      var total = codesAndQuantities.filter(x => x.absenceCode == key).reduce((a, b) => a + (b.quantity || 0), 0);
      return { absenceCode: key, description: codes[key].description, quantity: total };
    });
    const studentGeneralData =
      `<p>
            ${student.adaAbsences} Missed ADA (Days) <br>
            ${student.firstName} ${(student.middleName ? (student.middleName + ' ') : '')} ${student.lastSurname} <br>
            Student Unique Id: ${student.studentUniqueId} <br> 
            Address: ${generalData.streetNumberName} ${generalData.apartmentRoomSuiteNumber ? generalData.apartmentRoomSuiteNumber : ''} <br>
            ${generalData.city}, ${generalData.state} ${generalData.postalCode ? generalData.postalCode : ''}
            Campus: ${generalData.nameOfInstitution} <br>
            GPA: ${generalData.gpa}
      </p>`;
    const maxAbsencesInAPeriod = Math.max.apply(null, generalData.periods.map(x => x.absenceCodes.reduce((a, b) => a + (b.quantity || 0), 0)));
    const studentAbsenceCodeData =
      generalData.periods.map(x => {
        var mapCodesToBar = x.absenceCodes.map(x => {
          var percent = (((x.quantity || 0) / maxAbsencesInAPeriod) * 100);
          return `<div class="dna-code-${x.absenceCode} dna-code" style="width:${percent}%;">
                    ${x.quantity}
                 </div>
                 `;
        }).join('');
        return `<div class="row" style="margin:0">
                  <div class="col-2" style="padding:0">
                      ${x.classPeriodName}
                  </div>
                  <div class="col-10">
                    <div class="row" class="dna-bar-code">
                      ${mapCodesToBar}
                    </div>
                  </div>
              </div>`;
      }).join('');

    var tablerows = tableData.map(x => {
      return `
              <tr>
                <td>${x.absenceCode} <div class="dna-code-box dna-code-${x.absenceCode}"></div></td>
                <td>${x.description}</td>
                <td>${x.quantity}</td>
              </tr>
             `;
    }).join('');
    const totalTable =
      `<table class="table grid-table">
                <thead>
                    <tr>
                      <th>Code</th>
                      <th>Description</th>
                      <th>Absences</th>
                    </tr>
                </thead>
                <tbody>
                  ${tablerows}
                </tbody>
              </table>`;

    var result = studentGeneralData + studentAbsenceCodeData + totalTable;

    return result;
  }

  resetViewAndMarkers() {
    this.selectedMarker = null;
    this.markers.forEach(mymarker => {
      mymarker.circle.setMap(null);
      mymarker.marker.setMap(null);
      if (mymarker.clickListener) {
        mymarker.clickListener.remove();
      }
    });
  }

  sendRequest(search?: string) {
    if (this.mapElement)
      this.map = new google.maps.Map(this.mapElement.nativeElement, this.mapOptions);
    this.loading = true;
    this.resetViewAndMarkers();
    var gridRequest = new GridRequest(new Grid());
    if (search)
      gridRequest.searchTerm = search;
    gridRequest.filters = this.filters.filter(x => !this.nonValidFilters.some(f => f == x.column) && x.value && x.value != "undefined");
    this.apiService.student.getDnaChartData(gridRequest).subscribe((result: any) => {
      this.data = result.data;
      this.coordinatesHelper = {};
      this.data = this.data.map(x => {
        if (!x.latitude || !x.longitude || +x.latitude == 0 || +x.longitude == 0) { // If a student does not have coordinates send them to Purgatory Creek
          x.latitude = 29.882765;
          x.longitude = -97.935337;
        }
        else {
          if (!this.coordinatesHelper[x.latitude + '-' + x.longitude] && this.coordinatesHelper[x.latitude + '-' + x.longitude] !== 0)
            this.coordinatesHelper[x.latitude + '-' + x.longitude] = 0;
          else
            this.coordinatesHelper[x.latitude + '-' + x.longitude] += 2;

          x.latitude = Number(x.latitude) + (this.coordinatesHelper[x.latitude + '-' + x.longitude] / 1000);
          x.longitude = Number(x.longitude);
        }
        return x;
      });

      console.log(this.coordinatesHelper);
      this.noGeoDataStudents = this.data.filter(x => x.latitude == 29.882765 && x.longitude == -97.935337).length;
      this.initMarkers();
      this.loading = false;
    });
  }
}
export class MyMarker {
  marker: google.maps.Marker;
  infoWindow: google.maps.InfoWindow;
  clickListener: google.maps.MapsEventListener;
  circle: google.maps.Circle;
  student: any;
}

