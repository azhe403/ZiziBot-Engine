import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {WelcomeMessageComponent} from "./welcome-message/welcome-message.component";

const routes: Routes = [
    {
        path: '',
        component: WelcomeMessageComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class WelcomeMessageRoutingModule {
}
