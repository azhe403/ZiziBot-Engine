import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {GroupSelectorComponent} from './group-selector/group-selector.component';
import {DropdownModule} from "primeng/dropdown";
import {FormsModule} from "@angular/forms";
import {ProgressBarModule} from "primeng/progressbar";
import {ButtonModule} from "primeng/button";
import {TooltipModule} from "primeng/tooltip";


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
        ProgressBarModule,
        ButtonModule,
        TooltipModule
    ]
})
export class GroupSelectorModule {
}
