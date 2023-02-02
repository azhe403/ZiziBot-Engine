import { TestBed } from '@angular/core/testing';

import { AntispamService } from './antispam.service';

describe('AntispamService', () => {
  let service: AntispamService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AntispamService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
