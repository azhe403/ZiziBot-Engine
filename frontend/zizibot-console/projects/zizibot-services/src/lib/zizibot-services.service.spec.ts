import {TestBed} from '@angular/core/testing';

import {ZizibotServicesService} from './zizibot-services.service';

describe('ZizibotServicesService', () => {
    let service: ZizibotServicesService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(ZizibotServicesService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
