import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {MirrorUserRoutingModule} from './mirror-user-routing.module';
import {UserManagementComponent} from './user-management/user-management.component';
import {MatButtonModule} from "@angular/material/button";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatIconModule} from "@angular/material/icon";


@NgModule({
  declarations: [
    UserManagementComponent
  ],
  imports: [
    CommonModule,
    MirrorUserRoutingModule,
    MatButtonModule,
    MatToolbarModule,
    MatIconModule
  ]
})
export class MirrorUserModule { }
