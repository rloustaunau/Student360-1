import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';


@Injectable({providedIn: 'root'})
export class OAuthApiService {

    constructor(private http: HttpClient) { }

    public exchangeToken(request: OAuthTokenExchangeRequest) {
        return this.http.post<OAuthResponse>(`${environment.apiUrl}/oauth/exchangeToken`, request);
    }
}

export class OAuthTokenExchangeRequest {
    id_token: string;
    grant_type: string;
}

export class OAuthResponse {
    access_token: string;
    token_type: string;
    expires_in: number;
    refresh_token: string;
    example_parameter: string;
}
