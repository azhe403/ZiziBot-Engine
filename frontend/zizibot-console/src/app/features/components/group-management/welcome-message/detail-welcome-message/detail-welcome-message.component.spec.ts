import {ComponentFixture, TestBed} from '@angular/core/testing';

import {DetailWelcomeMessageComponent} from './detail-welcome-message.component';

describe('DetailWelcomeMessageComponent', () => {
  let component: DetailWelcomeMessageComponent;
  let fixture: ComponentFixture<DetailWelcomeMessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailWelcomeMessageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetailWelcomeMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
