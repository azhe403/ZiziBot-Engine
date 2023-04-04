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


@NgModule({
    declarations: [
        WelcomeMessageComponent
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
        RippleModule
    ]
})
export class WelcomeMessageModule {
}
