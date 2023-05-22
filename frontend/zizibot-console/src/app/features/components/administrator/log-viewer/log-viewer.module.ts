import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {LogViewerRoutingModule} from './log-viewer-routing.module';
import {LogViewerComponent} from './log-viewer/log-viewer.component';
import {TableModule} from "primeng/table";
import {DropdownModule} from "primeng/dropdown";
import {FormsModule} from "@angular/forms";

@NgModule({
  declarations: [
    LogViewerComponent
  ],
    imports: [
        CommonModule,
        LogViewerRoutingModule,
        TableModule,
        DropdownModule,
        FormsModule
    ]
})
export class LogViewerModule { }
