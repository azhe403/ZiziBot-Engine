import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {WelcomeComponent} from "./welcome/welcome.component";
import {AfterTelegramLoginComponent} from "./after-telegram-login/after-telegram-login.component";

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'angular', component: WelcomeComponent},
  {path: 'after-telegram-login', component: AfterTelegramLoginComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
