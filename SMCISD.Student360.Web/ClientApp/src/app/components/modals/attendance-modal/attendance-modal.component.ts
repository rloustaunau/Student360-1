import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component } from '@angular/core';


@Component({
    selector: 'app-attendance-modal',
    templateUrl: './attendance-modal.component.html'
})

export class AttendanceModalComponent implements OnInit {
    @Input() studentUsi: number;
    loading = true;
    data: GeneralStudentDnaData;
    codesAndQuantities: AbsencesCodesByPeriod[];
    tableData: AbsencesCodesByPeriod[];
    maxAbsencesInAPeriod: number;
    constructor(public activeModal: NgbActiveModal, private api: ApiService) {
        this.data = new GeneralStudentDnaData;
        this.codesAndQuantities = [];
        this.tableData = [];
        this.maxAbsencesInAPeriod = 100;

    }

    ngOnInit() {
        this.api.student.getStudentAbsencesCodesByPeriod(this.studentUsi).subscribe(result => {
            this.data = result;
            this.calculateData();
            this.loading = false;
        }, error => {
            this.loading = false;
        });
    }
    calculateData() {
        this.codesAndQuantities = [].concat.apply([], this.data.periods.map(x => x.absenceCodes));
        this.tableData = [];

        const codes = {};
        this.codesAndQuantities.forEach(x => {
            codes[x.absenceCode] = { description: x.description, absenceCode: x.absenceCode };
        });

        this.tableData = Object.keys(codes).map(key => {
            const total = this.codesAndQuantities.filter(x => x.absenceCode == key).reduce((a, b) => a + (b.quantity || 0), 0);
            return { absenceCode: key, description: codes[key].description, quantity: total };
        });

        this.maxAbsencesInAPeriod = Math.max.apply(null, this.data.periods
            .map(x => x.absenceCodes.reduce((a, b) => a + (b.quantity || 0), 0)));
    }

    getBarWidthPercent(quantity: number): number {
        return (((quantity || 0) / this.maxAbsencesInAPeriod) * 100);
    }
}