import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {GroupManagementRoutingModule} from "./group-management-routing.module";
import {MessagesModule} from "primeng/messages";
import {MessageModule} from "primeng/message";
import {ToastModule} from "primeng/toast";


@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        MessagesModule,
        MessageModule,
        ToastModule,
        GroupManagementRoutingModule
    ]
})
export class GroupManagementModule {
}
