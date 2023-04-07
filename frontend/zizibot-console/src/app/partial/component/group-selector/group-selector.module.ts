import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {GroupSelectorComponent} from './group-selector/group-selector.component';
import {DropdownModule} from "primeng/dropdown";
import {FormsModule} from "@angular/forms";
import {ProgressBarModule} from "primeng/progressbar";


@NgModule({
    declarations: [
        GroupSelectorComponent
    ],
    exports: [
        GroupSelectorComponent
    ],
    imports: [
        CommonModule,
        DropdownModule,
        FormsModule,
        ProgressBarModule
    ]
})
export class GroupSelectorModule {
}
