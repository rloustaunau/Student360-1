import { Component, Input } from '@angular/core';
import * as moment from 'moment';
import { DistrictDailyAttendanceRate } from '../../../app/services/api/report.service';
import { ApiService } from '../../../app/services/api/api.service';

@Component({
  selector: 'app-line-chart-example',
  templateUrl: './line-chart-example.component.html',
  styleUrls: ['./line-chart-example.component.css']
})

export class LineChartExampleComponent {
  data: DistrictDailyAttendanceRate[];
  chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scaleShowVerticalLines: false,
    legend: {
      display: true,
      position: 'top'
    },
    scales: {
      yAxes: [{
        display: true,
        ticks: {
          max: 8250,
          min: 5250,
          autoSkip: false
        },
        gridLines: {
          display: false
        }
      }],
      xAxes: [{
        gridLines: {
          display: false
        },
        type: 'time',
        time:
        {
          unit: 'day',
          displayFormats: { day: 'DD MMM YYYY' },
          //max: '12/12/2020 18:43:53',
          //min: '08/26/2020 18:43:53'
        }

      }],
    }
  };

  chartData = [{ data: [] }];

  chartLabels = [];

  constructor(private apiService: ApiService) {
    this.data = [];
  }

  ngOnInit() {
    this.apiService.report.getAdaRates().subscribe(result => {
      this.data = result;
      this.generateChartData();
    });
  }

  generateChartData() {
    this.chartData = [];
    var date = moment("20200826");
    var dataObject = {
      data: this.data.map(x => { return { t: moment(x.date).format('L'), y: Number(x.membership.replace(',', '')) } }),
      label: 'Membership',
      backgroundColor: 'transparent',
      pointBackgroundColor: 'rgba(110,211,207,1)',
      pointBorderColor: 'rgba(110,211,207,1)',
      borderColor: 'rgba(110,211,207,1)',
      pointRadius: 1.5,
      pointHoverRadius: 2,
      borderWidth: 2
    };

    var dataObject2 = {
      data: this.data.map(x => { return { t: moment(x.date).format('L'), y: Number(x.present.replace(',', '')) } }),
      label: 'PRESENT',
      backgroundColor: 'transparent',
      pointBackgroundColor: 'rgba(236, 102, 124, 1)',
      pointBorderColor: 'rgba(236, 102, 124, 1)',
      borderColor: 'rgba(236, 102, 124, 1)',
      pointRadius: 1.5,
      pointHoverRadius: 2,
      borderWidth: 2
    };

    this.chartData.push(dataObject);
    this.chartData.push(dataObject2);
    console.log(dataObject.data);
    console.log(dataObject2.data);
  }

  getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }
}

