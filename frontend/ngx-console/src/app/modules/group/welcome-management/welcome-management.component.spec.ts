import {ComponentFixture, TestBed} from '@angular/core/testing';

import {WelcomeManagementComponent} from './welcome-management.component';

describe('WelcomeManagementComponent', () => {
  let component: WelcomeManagementComponent;
  let fixture: ComponentFixture<WelcomeManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WelcomeManagementComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(WelcomeManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});