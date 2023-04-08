import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {WelcomeMessageRoutingModule} from './welcome-message-routing.module';
import {WelcomeMessageComponent} from './welcome-message/welcome-message.component';
import {ButtonModule} from "primeng/button";
import {DropdownModule} from "primeng/dropdown";
import {InputTextModule} from "primeng/inputtext";
import {MultiSelectModule} from "primeng/multiselect";
import {ProgressBarModule} from "primeng/progressbar";
import {SharedModule} from "primeng/api";
import {SliderModule} from "primeng/slider";
import {TableModule} from "primeng/table";
import {ToastModule} from "primeng/toast";
import {RippleModule} from "primeng/ripple";
import {DetailWelcomeMessageComponent} from './detail-welcome-message/detail-welcome-message.component';
import {ToolbarModule} from "primeng/toolbar";
import {AutoCompleteModule} from "primeng/autocomplete";
import {ReactiveFormsModule} from "@angular/forms";
import {CheckboxModule} from "primeng/checkbox";
import {InputTextareaModule} from "primeng/inputtextarea";
import {EditorModule} from "primeng/editor";
import {GroupSelectorModule} from "../../../../partial/component/group-selector/group-selector.module";
import {TooltipModule} from "primeng/tooltip";


@NgModule({
    declarations: [
        WelcomeMessageComponent,
        DetailWelcomeMessageComponent
    ],
    imports: [
        CommonModule,
        WelcomeMessageRoutingModule,
        ButtonModule,
        DropdownModule,
        InputTextModule,
        MultiSelectModule,
        ProgressBarModule,
        SharedModule,
        SliderModule,
        TableModule,
        ToastModule,
        RippleModule,
        ToolbarModule,
        AutoCompleteModule,
        ReactiveFormsModule,
        CheckboxModule,
        InputTextareaModule,
        EditorModule,
        GroupSelectorModule,
        TooltipModule
    ]
})
export class WelcomeMessageModule {
}
