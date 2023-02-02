import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {FedBanManagementComponent} from './fed-ban-management/fed-ban-management.component';
import {AuthGuard} from '../../guards/auth/auth.guard';

const routes: Routes = [
  {
    path: 'fed-ban-management',
    component: FedBanManagementComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AntispamRoutingModule {
}