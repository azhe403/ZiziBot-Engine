import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {MessageModule} from "primeng/message";
import {MessagesModule} from "primeng/messages";
import {ToastModule} from "primeng/toast";
import {GroupManagementRoutingModule} from "./group-management-routing.module";


@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        MessagesModule,
        MessageModule,
        ConfirmDialogModule,
        ToastModule,
        GroupManagementRoutingModule
    ]
})
export class GroupManagementModule {
}
