import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ZizibotCommonComponent } from './zizibot-common.component';

describe('ZizibotCommonComponent', () => {
  let component: ZizibotCommonComponent;
  let fixture: ComponentFixture<ZizibotCommonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ZizibotCommonComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ZizibotCommonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
