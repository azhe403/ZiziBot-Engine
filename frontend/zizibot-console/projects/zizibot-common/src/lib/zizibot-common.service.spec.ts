import { TestBed } from '@angular/core/testing';

import { ZizibotCommonService } from './zizibot-common.service';

describe('ZizibotCommonService', () => {
  let service: ZizibotCommonService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ZizibotCommonService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
