import {RouterModule, Routes} from "@angular/router";
import {AuthGuard} from "../../guards/auth/auth.guard";
import {NgModule} from "@angular/core";
import {WelcomeManagementComponent} from "./welcome-management/welcome-management.component";

const routes: Routes = [
  {
    path: 'welcome-management',
    component: WelcomeManagementComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GroupRoutingModule {
}