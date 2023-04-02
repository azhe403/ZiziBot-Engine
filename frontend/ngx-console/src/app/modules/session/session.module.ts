import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {SessionRoutingModule} from './session-routing.module';
import {VerifySessionComponent} from './verify-session/verify-session.component';


@NgModule({
  declarations: [
    VerifySessionComponent
  ],
  imports: [
    CommonModule,
    SessionRoutingModule
  ]
})
export class SessionModule {
}