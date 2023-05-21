import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {ReactiveFormsModule} from "@angular/forms";
import {SharedModule} from "primeng/api";
import {ButtonModule} from "primeng/button";
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {DataViewModule} from "primeng/dataview";
import {DividerModule} from "primeng/divider";
import {DropdownModule} from "primeng/dropdown";
import {FieldsetModule} from "primeng/fieldset";
import {InputTextModule} from "primeng/inputtext";
import {InputTextareaModule} from "primeng/inputtextarea";
import {PaginatorModule} from "primeng/paginator";
import {RippleModule} from "primeng/ripple";
import {TableModule} from "primeng/table";
import {ToastModule} from "primeng/toast";
import {ToolbarModule} from "primeng/toolbar";
import {TooltipModule} from "primeng/tooltip";
import {GroupSelectorModule} from "../../../../partial/component/group-selector/group-selector.module";
import {DetailNoteComponent} from './detail-note/detail-note.component';

import {NotesManagementRoutingModule} from './notes-management-routing.module';
import {NotesManagementComponent} from './notes-management/notes-management.component';
import {ListboxModule} from "primeng/listbox";
import {SplitterModule} from "primeng/splitter";


@NgModule({
    declarations: [
        NotesManagementComponent,
        DetailNoteComponent
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
        FieldsetModule,
        ConfirmDialogModule,
        InputTextModule,
        InputTextareaModule,
        PaginatorModule,
        ReactiveFormsModule,
        ListboxModule,
        SplitterModule
    ]
})
export class NotesManagementModule {
}
