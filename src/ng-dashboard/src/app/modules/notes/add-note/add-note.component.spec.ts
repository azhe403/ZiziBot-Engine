import {ComponentFixture, TestBed} from '@angular/core/testing';

import {AddNoteComponent} from './add-note.component';

describe('AddNoteComponent', () => {
  let component: AddNoteComponent;
  let fixture: ComponentFixture<AddNoteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddNoteComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AddNoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});