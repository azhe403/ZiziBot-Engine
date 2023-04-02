import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {AntispamRoutingModule} from './antispam-routing.module';
import {FedBanManagementComponent} from './fed-ban-management/fed-ban-management.component';
import {AgGridModule} from "ag-grid-angular";
import {MatCompoundModule} from '../partial/mat-compound.module';
import {AddBanComponent} from './add-ban/add-ban.component';


@NgModule({
  declarations: [
    FedBanManagementComponent,
    AddBanComponent
  ],
  imports: [
    CommonModule,
    AntispamRoutingModule,
    AgGridModule,
    MatCompoundModule
  ]
})
export class AntispamModule {
}