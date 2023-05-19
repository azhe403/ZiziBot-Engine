import {ComponentFixture, TestBed} from '@angular/core/testing';

import {DetailNoteComponent} from './detail-note.component';

describe('DetailNoteComponent', () => {
  let component: DetailNoteComponent;
  let fixture: ComponentFixture<DetailNoteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailNoteComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetailNoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
