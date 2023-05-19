import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {RssManagementRoutingModule} from './rss-management-routing.module';
import {ListRssComponent} from './list-rss/list-rss.component';
import {SplitterModule} from "primeng/splitter";
import {GroupSelectorModule} from "../../../../partial/component/group-selector/group-selector.module";
import {ButtonModule} from "primeng/button";
import {ToastModule} from "primeng/toast";
import {ToolbarModule} from "primeng/toolbar";
import {TooltipModule} from "primeng/tooltip";
import {ListboxModule} from "primeng/listbox";
import {FormsModule} from "@angular/forms";


@NgModule({
    declarations: [
        ListRssComponent
    ],
    imports: [
        CommonModule,
        RssManagementRoutingModule,
        SplitterModule,
        GroupSelectorModule,
        ButtonModule,
        ToastModule,
        ToolbarModule,
        TooltipModule,
        ListboxModule,
        FormsModule
    ]
})
export class RssManagementModule {
}
