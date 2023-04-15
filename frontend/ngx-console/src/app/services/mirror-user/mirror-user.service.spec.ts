import {TestBed} from '@angular/core/testing';

import {MirrorUserService} from './mirror-user.service';

describe('MirrorUserService', () => {
  let service: MirrorUserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MirrorUserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
