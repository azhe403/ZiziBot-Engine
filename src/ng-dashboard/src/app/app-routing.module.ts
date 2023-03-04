import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {WelcomeComponent} from "./welcome/welcome.component";
import {AfterTelegramLoginComponent} from "./after-telegram-login/after-telegram-login.component";

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'angular', component: WelcomeComponent},
  {path: 'after-telegram-login', component: AfterTelegramLoginComponent},
  {
    path: 'mirror-user',
        loadChildren: () => import('./modules/mirror-user/mirror-user.module').then(m => m.MirrorUserModule)
  },
  {
    path: 'antispam',
    loadChildren: () => import('./modules/antispam/antispam.module').then(m => m.AntispamModule)
  },
  {
    path: 'verify',
    loadChildren: () => import('./modules/session/session.module').then(m => m.SessionModule)
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {
}