import {NgModule} from "@angular/core";
import {BrowserModule} from "@angular/platform-browser";

import {AppRoutingModule} from "./app-routing.module";
import {WelcomeComponent} from "./welcome/welcome.component";
import {HomeComponent} from "./home/home.component";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {RootComponent} from "./root/root.component";
import {TelegramLoginWidgetComponent} from "./components/telegram-login-widget/telegram-login-widget.component";
import {dynamicScriptDirective} from "./directives/dynamic-script/dynamic-script.directive";
import {AfterTelegramLoginComponent} from "./after-telegram-login/after-telegram-login.component";
import {CookieService} from "ngx-cookie-service";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {AuthInterceptor} from "./interceptors/auth/auth.interceptor";
import {MatCompoundModule} from "./modules/partial/mat-compound.module";
import {ComponentModule} from "./components/component.module";
import {NgOptimizedImage} from "@angular/common";

@NgModule({
  declarations: [
    WelcomeComponent,
    HomeComponent,
    RootComponent,
    TelegramLoginWidgetComponent,
    dynamicScriptDirective,
    AfterTelegramLoginComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatCompoundModule,
    ComponentModule,
    NgOptimizedImage,
  ],
  providers: [
    CookieService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
  bootstrap: [RootComponent],
})
export class AppModule {
}