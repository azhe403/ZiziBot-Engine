import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { WelcomeMessageRoutingModule } from './welcome-message-routing.module';
import { WelcomeMessageComponent } from './welcome-message/welcome-message.component';


@NgModule({
  declarations: [
    WelcomeMessageComponent
  ],
  imports: [
    CommonModule,
    WelcomeMessageRoutingModule
  ]
})
export class WelcomeMessageModule { }
