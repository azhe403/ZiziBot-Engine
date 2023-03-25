import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {GroupSelectorComponent} from "./group-selector/group-selector.component";
import {MatCompoundModule} from "../modules/partial/mat-compound.module";

@NgModule({
  declarations: [
    GroupSelectorComponent
  ],
  exports: [
    GroupSelectorComponent
  ],
  imports: [
    CommonModule,
    MatCompoundModule
  ]
})
export class ComponentModule {
}