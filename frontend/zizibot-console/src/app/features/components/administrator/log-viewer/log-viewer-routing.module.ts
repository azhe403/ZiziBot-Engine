import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {LogViewerComponent} from "./log-viewer/log-viewer.component";
import {authGuard} from "../../../../core/auth.guard";

const routes: Routes = [
    {path: '', component: LogViewerComponent, canActivate: [authGuard], data: {role: 'Sudo'}}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LogViewerRoutingModule {
}
