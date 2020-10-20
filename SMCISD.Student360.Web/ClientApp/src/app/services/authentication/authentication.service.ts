import { Injectable } from '@angular/core';
import { ApiService } from '../api/api.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import * as jwt_decode from "jwt-decode";

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
   private currentUserSubject: BehaviorSubject<User>;
   private tokenInfoSubject: BehaviorSubject<any>;
   public currentUser: Observable<User>;
  public tokenInfo: Observable<Token>;

    constructor(private api: ApiService) {
      this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
      if (JSON.parse(localStorage.getItem('currentUser')))
        this.tokenInfoSubject = new BehaviorSubject<Token>(jwt_decode(JSON.parse(localStorage.getItem('currentUser')).token));
      else
        this.tokenInfoSubject = new BehaviorSubject<Token>( new Token);

      this.currentUser = this.currentUserSubject.asObservable();
      this.tokenInfo = this.tokenInfoSubject.asObservable();
    }

    public get currentTokenValue(): any {
      return this.tokenInfoSubject.value;
    }

    public get currentUserValue(): User {
        return this.currentUserSubject.value;
    }

    exchangeIdToken(idToken: string) {

        var request = new OAuthTokenExchangeRequest();
        request.id_token = idToken;
        request.grant_type = 'client_credentials';

        return this.api.oauth.exchangeToken(request).pipe(map(oAuthResponse => {
            // store user details and jwt token in local storage to keep user logged in between page refreshes
            // TODO: Decode the JWT and return basic User level props.
            var user = new User();
            user.token = oAuthResponse.access_token;

            localStorage.setItem('currentUser', JSON.stringify(user));
            this.currentUserSubject.next(user);
            this.tokenInfoSubject.next(jwt_decode(user.token));
            return user;
        }));
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }
}

export class User {
    public id: number;
    public firstName: string;
    public lastName: string;
    public token: string;
}

export class OAuthTokenExchangeRequest {
    id_token: string;
    grant_type: string;
}

export class Token {
  access_level: string;
  firstname: string;
  lastsurname: string;
  person_unique_id: string;
  person_usi: string;
  role: string;
  level_id: string;
}
