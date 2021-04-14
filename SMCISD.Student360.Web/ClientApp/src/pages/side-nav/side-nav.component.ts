import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../app/services/api/api.service';
import { AccessLevelDefinition } from '../../app/services/api/current-user.service';

@Component({
  selector: 'app-side-nav',
  templateUrl: './side-nav.component.html',
  styleUrls: ['./side-nav.component.css']
})
export class SideNavComponent implements OnInit {
  access: boolean;

  constructor(
    private router: Router,
    public apiService: ApiService,
    ) {
    }

  ngOnInit(): void {
    this.apiService.currentUser.getAccessLevel().subscribe(result => {
      if (result == null) {
        this.access = false;
      } else {
        this.access = true; 
      }
    });
  }


}
