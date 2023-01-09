import {ComponentFixture, TestBed} from '@angular/core/testing';

import {AddMirrorUserComponent} from './add-mirror-user.component';

describe('AddMirrorUserComponent', () => {
  let component: AddMirrorUserComponent;
  let fixture: ComponentFixture<AddMirrorUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddMirrorUserComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddMirrorUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
