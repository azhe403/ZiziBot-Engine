import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {ListRssComponent} from "./list-rss/list-rss.component";

const routes: Routes = [
    {
        path: '', component: ListRssComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RssManagementRoutingModule {
}
