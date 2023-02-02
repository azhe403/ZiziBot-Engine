import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FedBanManagementComponent } from './fed-ban-management.component';

describe('FedbanManagementComponent', () => {
  let component: FedBanManagementComponent;
  let fixture: ComponentFixture<FedBanManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FedBanManagementComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FedBanManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});