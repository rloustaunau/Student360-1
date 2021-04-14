import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneralStudentDnaData, AbsencesCodesByPeriod } from '@app/services/api/student.service';
import { ApiService } from '@app/services/api/api.service';
import { Input, OnInit, Component } from '@angular/core';


@Component({
  selector: 'app-message-modal',
  templateUrl: './message-modal.component.html'
})

export class MessageModalComponent implements OnInit {
  @Input() title: string;
  @Input() message: string;
  @Input() reasons: string[];
  constructor(public activeModal: NgbActiveModal) {

  }

  ngOnInit() {

  }
}
