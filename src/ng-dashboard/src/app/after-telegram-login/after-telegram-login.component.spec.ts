import {ComponentFixture, TestBed} from '@angular/core/testing';

import {AfterTelegramLoginComponent} from './after-telegram-login.component';

describe('AfterTelegramLoginComponent', () => {
  let component: AfterTelegramLoginComponent;
  let fixture: ComponentFixture<AfterTelegramLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AfterTelegramLoginComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AfterTelegramLoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
