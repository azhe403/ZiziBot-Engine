import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerifySessionComponent } from './verify-session.component';

describe('VerifySessionComponent', () => {
  let component: VerifySessionComponent;
  let fixture: ComponentFixture<VerifySessionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VerifySessionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VerifySessionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
