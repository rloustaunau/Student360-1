import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';

import { AuthenticationService } from '../services/authentication/authentication.service';
import { Router } from '@angular/router';
import { catchError, finalize } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  counter: number;
  constructor(private authenticationService: AuthenticationService, private router: Router, private toastr: ToastrService) {
    this.counter = 0;
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.counter++;
    // add authorization header with jwt token if available
    let currentUser = this.authenticationService.currentUserValue;
    if (currentUser && currentUser.token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      });
    }

    return next.handle(request).pipe(
      catchError(
        (err, caught) => {
          console.log(err);
          let errorMessage = '';

          if (err.error instanceof ErrorEvent) {
            // client-side error
            errorMessage = `Error: ${err.error.message}`;
          } else {
            // server-side error
            if (err.status == 401 && this.counter == 1) // Multiple requests can be made when the session has expired
              this.handleAuthError();


            if (err.error && err.error.Code != 500) // 500 errors shouldn't be displayed
              this.toastr.warning(err.error.Message);

            errorMessage = `Error Code: ${err.status}\nMessage: ${err.message}`;
          }

          return throwError(errorMessage);
        }
      ),
      finalize(() => {
        this.counter--;
      })
    );
  }

  private handleAuthError() {
    this.toastr.info('Your session has expired.');
    localStorage.getItem('currentUser');
    localStorage.removeItem('currentUser');
    this.router.navigateByUrl('login');

  }
}
