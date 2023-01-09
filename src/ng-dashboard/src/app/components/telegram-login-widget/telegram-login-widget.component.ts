import {AfterViewInit, Component} from '@angular/core';
import {TelegramUserLogin} from "../../types/TelegramUserLogin";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-telegram-login-widget',
  templateUrl: './telegram-login-widget.component.html',
  styleUrls: ['./telegram-login-widget.component.scss']
})
export class TelegramLoginWidgetComponent implements AfterViewInit{
  botName = environment.botName;

  constructor() {

  }

  onTelegramAuth(user: TelegramUserLogin) {
    alert('Logged in as ' + user.first_name + ' ' + user.last_name + ' (' + user.id + (user.username ? ', @' + user.username : '') + ')');
  }

  ngAfterViewInit(): void {
    this.loadWidgetLogin();
  }

  loadWidgetLogin(){
    const loginWidget = document.getElementById('telegram-login-widget');
    // @ts-ignore
    loginWidget.setAttribute('data-telegram-login', this.botName);
  }

}
