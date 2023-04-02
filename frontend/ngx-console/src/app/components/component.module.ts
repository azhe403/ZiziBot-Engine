import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {GroupSelectorComponent} from "./group-selector/group-selector.component";
import {MatCompoundModule} from "../modules/partial/mat-compound.module";
import {AgGridExComponent} from './ag-grid-ex/ag-grid-ex.component';
import {AgGridModule} from "ag-grid-angular";

@NgModule({
  declarations: [
    GroupSelectorComponent,
    AgGridExComponent
  ],
  exports: [
    GroupSelectorComponent
  ],
  imports: [
    CommonModule,
    MatCompoundModule,
    AgGridModule
  ]
})
export class ComponentModule {
}