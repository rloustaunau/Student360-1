import { TestBed } from '@angular/core/testing';

import { OAuthApiService } from './o-auth-api.service';

describe('OAuthApiService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OAuthApiService = TestBed.get(OAuthApiService);
    expect(service).toBeTruthy();
  });
});
