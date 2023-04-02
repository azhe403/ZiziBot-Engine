import {ComponentFixture, TestBed} from '@angular/core/testing';

import {IniLibComponent} from './ini-lib.component';

describe('IniLibComponent', () => {
    let component: IniLibComponent;
    let fixture: ComponentFixture<IniLibComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [IniLibComponent]
        })
            .compileComponents();

        fixture = TestBed.createComponent(IniLibComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
