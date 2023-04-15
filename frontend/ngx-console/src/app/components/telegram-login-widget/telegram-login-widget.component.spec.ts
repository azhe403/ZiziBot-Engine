import {ComponentFixture, TestBed} from '@angular/core/testing';

import {TelegramLoginWidgetComponent} from './telegram-login-widget.component';

describe('TelegramLoginWidgetComponent', () => {
  let component: TelegramLoginWidgetComponent;
  let fixture: ComponentFixture<TelegramLoginWidgetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TelegramLoginWidgetComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(TelegramLoginWidgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});