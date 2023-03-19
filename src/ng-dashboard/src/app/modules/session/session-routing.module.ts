import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {VerifySessionComponent} from './verify-session/verify-session.component';

const routes: Routes = [
    {
        path: 'session/:sessionId',
        component: VerifySessionComponent
    },
    {
        path: 'verify',
        component: VerifySessionComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SessionRoutingModule {
}