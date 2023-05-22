import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LogViewerRoutingModule } from './log-viewer-routing.module';
import { LogViewerComponent } from './log-viewer/log-viewer.component';
import {TableModule} from "primeng/table";

@NgModule({
  declarations: [
    LogViewerComponent
  ],
    imports: [
        CommonModule,
        LogViewerRoutingModule,
        TableModule
    ]
})
export class LogViewerModule { }
