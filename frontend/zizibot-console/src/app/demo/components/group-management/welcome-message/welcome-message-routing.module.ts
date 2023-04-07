import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {WelcomeMessageComponent} from "./welcome-message/welcome-message.component";
import {DetailWelcomeMessageComponent} from "./detail-welcome-message/detail-welcome-message.component";

const routes: Routes = [
    {path: '', component: WelcomeMessageComponent},
    {path: 'add', component: DetailWelcomeMessageComponent},
    {path: ':welcomeId', component: DetailWelcomeMessageComponent},
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class WelcomeMessageRoutingModule {
}
