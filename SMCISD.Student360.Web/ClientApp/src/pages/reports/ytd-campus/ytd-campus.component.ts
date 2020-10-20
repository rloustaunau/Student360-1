import { Component, Input, ViewChild, ElementRef } from '@angular/core';
import * as moment from 'moment';
import { ChartOptions } from 'chart.js';
import * as annotations from 'chartjs-plugin-annotation';
import { BaseChartDirective } from 'ng2-charts';
import { YtdRegionRate } from '../../../app/services/api/report.service';
import { ApiService } from '../../../app/services/api/api.service';

@Component({
  selector: 'app-ytd-campus',
  templateUrl: './ytd-campus.component.html',
  styleUrls: ['./ytd-campus.component.css']
})

export class YtdCampusComponent {
  data: YtdRegionRate[];
  loading = false;
  @ViewChild('chart', { static: true }) chartref: ElementRef;
  chartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    //scaleShowVerticalLines: false,
    legend: {
      display: false,
      position: 'top'
    },
    scales: {
      xAxes: [{
        display: true,
        stacked: true,
        ticks: {
          max: 100,
          min: 80,
          autoSkip: false,
          stepSize: 1,
          callback: label => `${label}%`
        },
        gridLines: {
          display: true,
          color: 'white'
        },
        scaleLabel: {
          display: true,
          fontColor: 'white',
        }
      }],
      yAxes: [{
        display: true,
        gridLines: {
          display: false
        },
        stacked: true,
        ticks: {
          beginAtZero: true
        },
        scaleLabel: {
          display: true,
          fontColor: 'white',  
        }
      }],
    },
    tooltips: {
      callbacks: {
        label: (tooltipItem, data) => `${tooltipItem.value.toString().split('.')[0]
           + '.' + tooltipItem.value.toString().split('.')[1].substring(0, tooltipItem.value.toString().split('.')[1].length > 1 ? 2 : 1)}%`
      }
    },
    annotation: {
      annotations: [
        {
          type: "line",
          mode: "vertical",
          scaleID: "x-axis-0",
          value: 95,
          borderColor: "red",
          backgroundColor:"red",
          label: {
            content: "Limit",
            enabled: false,
            position: "top"
          }
        }
      ]
    }
  };

  chartData = [{data: []}];
  chartLabels = [];

  colors = [
    {
      backgroundColor: [],
      borderColor: [],
    }
  ];

  constructor(private apiService: ApiService) {
    BaseChartDirective.registerPlugin(annotations);
    this.data = [];
  }

  ngOnInit() {
    this.loading = true;
    this.apiService.report.getYtdRates().subscribe(result => {
      this.data = result;
      this.generateChartData();
      this.loading = false;
    });
  }

  generateColors() {
    let values = this.data.map(x => x.attendancePercent);

    values.forEach(value => {
      this.colors[0].backgroundColor.push('rgba(94, 100, 94,0.5)');
        this.colors[0].borderColor.push('rgba(94, 100, 94,1)');
    });
  }

  generateChartData() {
    this.chartData = [];
    this.chartLabels = [];
    this.generateColors();
    this.chartLabels = this.data.map(x => x.nameOfInstitution + ' ');
    var dataObject = {
      data: this.data.map(x => x.attendancePercent),
      label: 'Attendance',
      backgroundColor: 'transparent',
      pointBackgroundColor: 'rgba(110,211,207,1)',
      pointBorderColor: 'rgba(110,211,207,1)',
      borderColor: 'rgba(110,211,207,1)',
      pointRadius: 1.5,
      pointHoverRadius: 2,
      borderWidth: 2
    };
    var limitLineObject = {
      data: new Array(12).fill(95),
      label: 'Limit',
      type: 'line',
      backgroundColor:'red',
    }
    //var dataObject2 = {
    //  data: this.data.map(x => { return { t: moment(x.date).format('L'), y: Number(x.present.replace(',', '')) } }),
    //  label: 'PRESENT',
    //  backgroundColor: 'transparent',
    //  pointBackgroundColor: 'rgba(236, 102, 124, 1)',
    //  pointBorderColor: 'rgba(236, 102, 124, 1)',
    //  borderColor: 'rgba(236, 102, 124, 1)',
    //  pointRadius: 1.5,
    //  pointHoverRadius: 2,
    //  borderWidth: 2
    //};
    this.chartData.push(dataObject);
    this.chartData.push(limitLineObject);
  }
  getColorWidth(currentWidth) {
    return currentWidth - 165;
  }
  
}

