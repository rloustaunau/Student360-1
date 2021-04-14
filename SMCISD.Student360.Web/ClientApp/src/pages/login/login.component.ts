import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService, GoogleLoginProvider  } from "angularx-social-login";
import { ToastrService } from 'ngx-toastr';
import { AccessToSystem, CurrentUserApiService } from '../../app/services/api/current-user.service';
import { AuthenticationService } from '../../app/services/authentication';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

    loading = false;
    returnUrl: string;
    register= false;
    public model = {
        loggedIn: false,
        user: {}
    };

    constructor(
        private socialAuthService: AuthService,
        private authenticationService: AuthenticationService,
        private route: ActivatedRoute,
      private router: Router,
      private access: AccessToSystem,
      private currentUserService: CurrentUserApiService,
      private toastr: ToastrService) { }

    ngOnInit() {
        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.model.user = null;
    
        this.socialAuthService.authState.subscribe((user) => {
            if (!user)
                return;

          this.model.user = user;
          this.access = new AccessToSystem();
          this.access.email = user.email;
          this.access.fullName = user.name;

          if (!this.register) {
            this.currentUserService.createAccess(this.access).subscribe(data => {
              this.register = true; 
            });
          }

            this.model.loggedIn = (user != null);

            //authenticationService
            this.authenticationService.exchangeIdToken(user.idToken).subscribe(
              data => {
                this.router.navigate([this.returnUrl]);
              },
                error => {
                    this.loading = false;
                });
        });
    }

    signIn(): void {
      this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID); 
    }

    signOut(): void {
        this.socialAuthService.signOut(true).then(result => { localStorage.clear();});
    }

}
