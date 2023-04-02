import {ComponentFixture, TestBed} from '@angular/core/testing';

import {AddBanComponent} from './add-ban.component';

describe('AddBanComponent', () => {
  let component: AddBanComponent;
  let fixture: ComponentFixture<AddBanComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddBanComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AddBanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});