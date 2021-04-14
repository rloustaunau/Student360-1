import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Input, OnInit, Component } from '@angular/core';


@Component({
  selector: 'app-message-result-modal',
  templateUrl: './message-result-modal.component.html'
})

export class MessageResultModalComponent implements OnInit {
  @Input() title: string;
  @Input() successButtonName: string = "Ok";
  @Input() message: string;
  @Input() showSucessButton: boolean = true;

  constructor(public activeModal: NgbActiveModal) {

  }

  ngOnInit() {

  }
}
