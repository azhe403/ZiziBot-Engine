import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ZizibotServicesComponent} from './zizibot-services.component';

describe('ZizibotServicesComponent', () => {
    let component: ZizibotServicesComponent;
    let fixture: ComponentFixture<ZizibotServicesComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ZizibotServicesComponent]
        })
            .compileComponents();

        fixture = TestBed.createComponent(ZizibotServicesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
