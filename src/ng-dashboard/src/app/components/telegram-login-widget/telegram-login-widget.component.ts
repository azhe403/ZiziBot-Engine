import {Component} from '@angular/core';
import {TelegramUserLogin} from "../../types/TelegramUserLogin";

@Component({
  selector: 'app-telegram-login-widget',
  templateUrl: './telegram-login-widget.component.html',
  styleUrls: ['./telegram-login-widget.component.scss']
})
export class TelegramLoginWidgetComponent {

  onTelegramAuth(user: TelegramUserLogin) {
    alert('Logged in as ' + user.first_name + ' ' + user.last_name + ' (' + user.id + (user.username ? ', @' + user.username : '') + ')');
  }

}
