import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {UserManagementComponent} from "./user-management/user-management.component";
import {AuthGuard} from "../../guards/auth/auth.guard";

const routes: Routes = [
    {
        path: 'management',
        component: UserManagementComponent,
        canActivate: [AuthGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class MirrorUserRoutingModule {
}