import { TestBed } from '@angular/core/testing';

import { Courseforclaims } from './courseforclaims';

describe('Courseforclaims', () => {
  let service: Courseforclaims;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Courseforclaims);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
