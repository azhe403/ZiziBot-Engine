import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotesManagementComponent } from './notes-management.component';

describe('NotesManagementComponent', () => {
  let component: NotesManagementComponent;
  let fixture: ComponentFixture<NotesManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NotesManagementComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotesManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
