import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {SharedModule} from "primeng/api";
import {ButtonModule} from "primeng/button";
import {DataViewModule} from "primeng/dataview";
import {DividerModule} from "primeng/divider";
import {DropdownModule} from "primeng/dropdown";
import {FieldsetModule} from "primeng/fieldset";
import {RippleModule} from "primeng/ripple";
import {TableModule} from "primeng/table";
import {ToastModule} from "primeng/toast";
import {ToolbarModule} from "primeng/toolbar";
import {TooltipModule} from "primeng/tooltip";
import {GroupSelectorModule} from "../../../../partial/component/group-selector/group-selector.module";

import {NotesManagementRoutingModule} from './notes-management-routing.module';
import {NotesManagementComponent} from './notes-management/notes-management.component';


@NgModule({
    declarations: [
        NotesManagementComponent
    ],
    imports: [
        CommonModule,
        NotesManagementRoutingModule,
        ButtonModule,
        GroupSelectorModule,
        RippleModule,
        SharedModule,
        TableModule,
        ToastModule,
        ToolbarModule,
        TooltipModule,
        DataViewModule,
        DropdownModule,
        DividerModule,
        FieldsetModule
    ]
})
export class NotesManagementModule {
}
