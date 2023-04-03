import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

const routes: Routes = [
    {
        path: 'welcome-message',
        loadChildren: () => import('./welcome-message/welcome-message.module').then(m => m.WelcomeMessageModule)
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GroupManagementRoutingModule {
}
