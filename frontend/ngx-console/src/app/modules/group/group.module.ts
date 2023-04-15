import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {WelcomeManagementComponent} from './welcome-management/welcome-management.component';
import {CardModule} from "primeng/card";
import {GroupRoutingModule} from "./group-routing.module";
import {ComponentModule} from "../../components/component.module";
import {DataViewModule} from "primeng/dataview";


@NgModule({
  declarations: [
    WelcomeManagementComponent
  ],
  imports: [
    CommonModule,
    GroupRoutingModule,
    CardModule,
    ComponentModule,
    DataViewModule
  ]
})
export class GroupModule {
}