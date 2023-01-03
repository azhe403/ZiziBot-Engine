import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {WelcomeComponent} from './welcome/welcome.component';
import {HomeComponent} from './home/home.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MatButtonModule} from "@angular/material/button";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatIconModule} from "@angular/material/icon";
import {RootComponent} from './root/root.component';
import {MatMenuModule} from "@angular/material/menu";
import {MatSidenavModule} from "@angular/material/sidenav";
import {MatListModule} from "@angular/material/list";
import {TelegramLoginWidgetComponent} from './components/telegram-login-widget/telegram-login-widget.component';
import {dynamicScriptDirective} from "./directives/dynamic-script/dynamic-script.directive";

@NgModule({
    declarations: [
        WelcomeComponent,
        HomeComponent,
        RootComponent,
        TelegramLoginWidgetComponent,
        dynamicScriptDirective
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        MatButtonModule,
        MatToolbarModule,
        MatIconModule,
        MatMenuModule,
        MatSidenavModule,
        MatListModule
    ],
  providers: [],
  bootstrap: [RootComponent]
})
export class AppModule {
}
