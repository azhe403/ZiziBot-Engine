import {TestBed} from '@angular/core/testing';

import {IniLibService} from './ini-lib.service';

describe('IniLibService', () => {
    let service: IniLibService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(IniLibService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
