import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgGridExComponent } from './ag-grid-ex.component';

describe('AgGridExComponent', () => {
  let component: AgGridExComponent;
  let fixture: ComponentFixture<AgGridExComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AgGridExComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AgGridExComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
