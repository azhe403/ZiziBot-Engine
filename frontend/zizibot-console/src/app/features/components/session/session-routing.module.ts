import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {VerifyComponent} from "./verify/verify.component";

const routes: Routes = [
    {
        path: 'verify',
        component: VerifyComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SessionRoutingModule {
}
