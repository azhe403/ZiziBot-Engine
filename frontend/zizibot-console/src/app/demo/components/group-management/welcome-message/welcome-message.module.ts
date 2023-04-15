import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {ReactiveFormsModule} from "@angular/forms";
import {SharedModule} from "primeng/api";
import {AutoCompleteModule} from "primeng/autocomplete";
import {ButtonModule} from "primeng/button";
import {CheckboxModule} from "primeng/checkbox";
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {DropdownModule} from "primeng/dropdown";
import {EditorModule} from "primeng/editor";
import {InputTextModule} from "primeng/inputtext";
import {InputTextareaModule} from "primeng/inputtextarea";
import {MultiSelectModule} from "primeng/multiselect";
import {ProgressBarModule} from "primeng/progressbar";
import {RippleModule} from "primeng/ripple";
import {SliderModule} from "primeng/slider";
import {TableModule} from "primeng/table";
import {ToastModule} from "primeng/toast";
import {ToolbarModule} from "primeng/toolbar";
import {TooltipModule} from "primeng/tooltip";
import {GroupSelectorModule} from "../../../../partial/component/group-selector/group-selector.module";
import {DetailWelcomeMessageComponent} from './detail-welcome-message/detail-welcome-message.component';

import {WelcomeMessageRoutingModule} from './welcome-message-routing.module';
import {WelcomeMessageComponent} from './welcome-message/welcome-message.component';


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
        TooltipModule,
        ConfirmDialogModule
    ]
})
export class WelcomeMessageModule {
}
